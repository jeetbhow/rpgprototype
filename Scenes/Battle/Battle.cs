using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Signal;
using Combat.UI;
using Combat.Actors;

public partial class Battle : Node2D
{
    public enum PlayerState
    {
        Talking,
        Attacking,
    }

    [Signal] public delegate void BattleReadyEventHandler();

    [Export] public Array<Ally> Party { get; private set; }
    [Export] public Array<Enemy> Enemies { get; private set; }
    [Export] public PackedScene PartyInfoPanelScene { get; set; }
    [Export] public PackedScene EnemyNodeScene { get; set; }

    public BattleUI UI { get; private set; }
    public List<EnemyNode> EnemyNodes { get; private set; } = [];
    public Queue<Fighter> TurnQueue { get; set; } = new();
    public Fighter CurrFighter { get; set; }
    public Fighter FighterTargetedByPlayer { get; set; }
    public PlayerState CurrPlayerState { get; set; }

    private Camera2D _camera;
    private float _duration = 0.0f;
    private float _shakeTime = 0.0f;
    private float _initialShakeMagnitude = 0.0f;

    public void ShakeCamera(float duration, float magnitude)
    {
        _duration = duration;
        _shakeTime = duration;
        _initialShakeMagnitude = magnitude;
    }

    public override void _Ready()
    {
        _ = AsyncReady();
    }

    public async Task AsyncReady()
    {
        SignalHub.FighterAttacked += OnFighterAttacked;

        UI = GetNode<BattleUI>("BattleUI");
        _camera = GetNode<Camera2D>("Camera2D");

        try
        {
            Enemies.ToList().ForEach(enemy => enemy.InitializeHPAndAP());
            Party.ToList().ForEach(member => member.InitializeHPAndAP());

            SetupPartyInfoPanels();
            await SetupEnemyNodes();
            await DetermineTurnOrder();
            EmitSignal(SignalName.BattleReady);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to initialize battle: {e}");
        }
    }

    public override void _Process(double delta)
    {
        // Camera Shake
        if (_shakeTime > 0.0f)
        {
            _shakeTime -= (float)delta;

            float t = _shakeTime / _duration;
            float magnitude = Mathf.Lerp(_initialShakeMagnitude, 0.0f, 1 - t);

            float angle = GD.Randf() * Mathf.Tau;

            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
            UI.Offset = offset;
            _camera.Offset = offset / 2.0f;
        }
        else
        {
            UI.Offset = Vector2.Zero;
            _camera.Offset = Vector2.Zero;
        }
    }

    public void SetupPartyInfoPanels()
    {
        foreach (Ally ally in Party)
        {
            PartyInfoPanel panel = PartyInfoPanelScene.Instantiate<PartyInfoPanel>();
            panel.PartyMember = ally;
            UI.AddPartyInfoPanel(panel: panel);
        }
    }

    public async Task SetupEnemyNodes()
    {
        foreach (Enemy enemy in Enemies)
        {
            var node = EnemyNodeScene.Instantiate<EnemyNode>();
            node.EnemyData = enemy;
            EnemyNodes.Add(node);
        }

        var enemyNodesContainer = GetNode<Node2D>("EnemyNodesContainer");
        foreach (EnemyNode node in EnemyNodes)
        {
            enemyNodesContainer.AddChild(node);
            await UI.Log.AppendLine(node.EnemyData.IntroLog);
            await node.Introduction();
        }
    }

    public void OnFighterAttacked(FighterEventArgs args)
    {
        if (args.Defender is Player && args.Attack.HasDamage)
        {
            ShakeCamera(1.0f, 8.0f);
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt);
        }
    }

    public async Task DetermineTurnOrder()
    {
        Fighter[] allParticipants = new Fighter[Party.Count + Enemies.Count];
        Party.Cast<Fighter>().ToArray().CopyTo(allParticipants, 0);
        Enemies.Cast<Fighter>().ToArray().CopyTo(allParticipants, Party.Count);

        List<DiceRollInfo> tqPanels = [];

        foreach (Fighter f in allParticipants)
        {
            RandomNumberGenerator rng = new();
            int d1 = rng.RandiRange(Game.D6Min, Game.D6Max);
            int d2 = rng.RandiRange(Game.D6Min, Game.D6Max);

            f.Initiative = d1 + d2 + f.Athletics.Value;
            UI.CreateDiceRollInfo(f, d1, d2, SkillType.Athletics, f.Athletics.Value);

            await UI.Log.AppendLine($"{f.Name} rolled {f.Initiative} [color={Game.BodySkillColor.ToHtml()}](+{f.Athletics.Value})[/color] on initiative.");
        }

        await Game.Instance.Wait(500);
        TurnQueue = new Queue<Fighter>(allParticipants.OrderByDescending(p => p.Initiative));

        foreach (Fighter f in TurnQueue)
        {
            UI.AddTQPanel(f);
        }
    }

    public EnemyNode GetEnemyNode(int index)
    {
        return EnemyNodes[index];
    }

    public EnemyNode GetEnemyNode(Enemy enemy)
    {
        return EnemyNodes.FirstOrDefault(e => e.EnemyData == enemy);
    }
}
