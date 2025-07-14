using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Combat;

public partial class Battle : Node2D
{
    [Signal]
    public delegate void BattleReadyEventHandler();

    [Signal]
    public delegate void TurnEndEventHandler(Fighter f);

    [Export]
    public Array<Enemy> Enemies { get; private set; }

    [Export]
    public PackedScene PartyInfoPanelScene { get; set; }

    [Export]
    public PackedScene EnemyBattleSpriteScene { get; set; }

    public Node2D EnemyNodes { get; private set; }
    public BattleUI UI { get; private set; }
    public List<Ally> Party { get; private set; } = [];
    public Queue<Fighter> TurnQueue { get; set; } = new();
    public Fighter CurrFighter { get; set; }

    // Camera Parameters.
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
        SignalHub.Instance.FighterAttacked += OnFighterAttacked;

        UI = GetNode<BattleUI>("BattleUI");
        EnemyNodes = GetNode<Node2D>("EnemyNodes");
        _camera = GetNode<Camera2D>("Camera2D");

        try
        {
            InitParty();
            await InitEnemies();
            await DetermineTurnOrder();
            EmitSignal(SignalName.BattleReady);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to initialize battle: {e}");
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

    /// <summary>
    /// Initializes the party by creating Fighter instances for each ally in the party
    /// and updates the UI with their information.
    /// </summary>
    public void InitParty()
    {
        foreach (Ally a in Game.Instance.Party)
        {
            PartyInfoPanel panel = PartyInfoPanelScene.Instantiate<PartyInfoPanel>();
            panel.PartyMember = a;
            UI.AddPartyInfoPanel(panel);
            Party.Add(a);
        }
    }

    /// <summary>
    /// Initializes the enemies in the battle by iterating through the EnemyNodes.
    /// For each BattleEnemy node, it creates a Fighter instance and adds it to the Enemies list.
    /// It also plays a fade-in animation for each enemy and logs their introduction text
    /// to the UI log.
    /// </summary>
    public async Task InitEnemies()
    {
        foreach (Enemy e in Enemies)
        {
            var sprite = EnemyBattleSpriteScene.Instantiate<EnemyBattleSprite>();
            sprite.Enemy = e;
            EnemyNodes.AddChild(sprite);

            await UI.Log.AppendLine(e.IntroLog);
            await sprite.Introduction();
        }
    }

    public void OnFighterAttacked(Fighter attacker, Fighter defender, Ability ability)
    {
        if (defender is Player && ability.DamageRange != null)
        {
            ShakeCamera(1.0f, 8.0f);
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt);
        }
    }

    /// <summary>
    /// Determines the turn order for the battle participants based on their initiative.
    /// Initiative is calculated by rolling two six-sided dice (D6) and adding the Athletics
    /// skill of each participant.
    /// The turn order is stored in the TurnQueue, which is a queue of Fighters sorted
    /// in descending order of their initiative.
    /// </summary>
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

            f.Initiative = d1 + d2 + f.Athletics;
            UI.CreateDiceRollInfo(f, d1, d2, SkillType.Athletics, f.Athletics);

            await UI.Log.AppendLine($"{f.Name} rolled {f.Initiative} [color={Game.BodySkillColor.ToHtml()}](+{f.Athletics})[/color] on initiative.");
        }

        await Game.Instance.Wait(500);
        TurnQueue = new Queue<Fighter>(allParticipants.OrderByDescending(p => p.Initiative));

        foreach (Fighter f in TurnQueue)
        {
            UI.AddTQPanel(f);
        }
    }

    /// <summary>
    /// Retrieves the EnemyBattleSprite at the specified index.
    /// The index corresponds to the position of the child node in the EnemyNodes node.
    /// </summary>
    /// <param name="index">The position of the enemy in the EnemyNodes node.</param>
    /// <returns>The EnemyBattleSprite at the specified index.</returns>
    public EnemyBattleSprite GetEnemySprite(int index)
    {
        return EnemyNodes.GetChild<EnemyBattleSprite>(index, false);
    }
}
