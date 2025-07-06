using Godot;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


public partial class Battle : Node2D
{
    private const int _D6Min = 1;
    private const int _D6Max = 6;

    [Signal] public delegate void ActionBarFullEventHandler(int id);

    [Export] public PackedScene PartyInfoPanelScene { get; set; }

    public Node2D EnemyNodes { get; private set; }
    public BattleUI UI { get; private set; }
    public List<Ally> Party { get; private set; } = [];
    public List<Enemy> Enemies { get; private set; } = [];
    public Queue<Fighter> TurnQueue { get; set; } = new();
    public Fighter CurrFighter { get; set; }

    /// <summary>
    /// Initializes the Battle scene by setting up the UI and loading the enemies.
    /// </summary>
    /// <returns></returns>
    public async Task Init()
    {
        UI = GetNode<BattleUI>("BattleUI");
        EnemyNodes = GetNode<Node2D>("EnemyNodes");

        UI.Init();
        InitParty();
        await InitEnemies();
    }

    /// <summary>
    /// Initializes the party by creating Fighter instances for each ally in the party
    /// and updates the UI with their information.
    /// </summary>
    public void InitParty()
    {
        foreach (Ally ally in Game.Instance.Party)
        {
            PartyInfoPanel panel = PartyInfoPanelScene.Instantiate<PartyInfoPanel>();
            UI.AddPartyInfoPanel(panel);

            panel.PartyMemberName = ally.Name;
            panel.HP = ally.HP;
            panel.MaxHP = ally.MaxHP;
            panel.AP = ally.AP;
            panel.MaxAP = ally.MaxAP;

            Party.Add(ally);
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
        foreach (var enemy in EnemyNodes.GetChildren())
        {
            if (enemy is EnemyBattleSprite e)
            {
                Enemies.Add(e.Data);

                await UI.Log.AppendLine(e.Data.Introduction);
            }
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

        foreach (Fighter fighter in allParticipants)
        {
            RandomNumberGenerator rng = new();
            int d1 = rng.RandiRange(_D6Min, _D6Max);
            int d2 = rng.RandiRange(_D6Min, _D6Max);

            fighter.Initiative = d1 + d2 + fighter.Athletics;
            UI.CreateDiceRollInfo(fighter, d1, d2, SkillType.Athletics, fighter.Athletics);

            await UI.Log.AppendLine($"{fighter.Name} rolled {fighter.Initiative} [color={Game.BodySkillColor.ToHtml()}](+{fighter.Athletics} Athletics)[/color] on initiative.");
        }

        await Wait(500);
        TurnQueue = new Queue<Fighter>(allParticipants.OrderByDescending(p => p.Initiative));
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

    /// <summary>
    /// Damages the enemy at the specified index by the given amount. The index corresponds
    /// to the position of the child node in the EnemyNodes node, which is also the 
    /// position of the enemy in the Enemies list.
    /// </summary>
    /// <param name="index">The index of the enemy to damage.</param>
    /// <param name="damage">The amount of damage to deal.</param>
    public void DamageEnemyHP(int index, int damage)
    {
        var sprite = EnemyNodes.GetChild<EnemyBattleSprite>(index, false);
        sprite?.TakeDamage(damage);
        Enemies[index].HP -= damage;
    }

    public void DamageAllyHP(int index, int damage)
    {
        if (index < 0 || index >= Party.Count)
        {
            GD.PrintErr($"Invalid ally index: {index}");
            return;
        }

        var ally = Party[index];
        ally.HP -= damage;

        // Update the UI panel for the ally
        PartyInfoPanel panel = UI.GetPartyInfoPanel(index);
        if (panel != null)
        {
            double finalVal = Mathf.Clamp(ally.HP, 0, ally.MaxHP);

            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(panel, "HP", finalVal, 1.0f)
                 .SetTrans(Tween.TransitionType.Sine)
                 .SetEase(Tween.EaseType.Out);
        }
    }

    public void DamageAllyAP(int index, int damage)
    {
        if (index < 0 || index >= Party.Count)
        {
            GD.PrintErr($"Invalid ally index: {index}");
            return;
        }

        var ally = Party[index];
        ally.AP -= damage;

        // Update the UI panel for the ally
        PartyInfoPanel panel = UI.GetPartyInfoPanel(index);
        if (panel != null)
        {
            double finalVal = Mathf.Clamp(ally.AP, 0, ally.MaxAP);

            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(panel, "AP", finalVal, 1.0f)
                 .SetTrans(Tween.TransitionType.Sine)
                 .SetEase(Tween.EaseType.Out);
        }
    }

    /// <summary>
    /// Updates the Action Points (AP) of the specified fighter by subtracting the given AP cost.
    /// </summary>
    /// <param name="fighter">The fighter that's being updated.</param>
    /// <param name="apCost">The amount of AP to subtract.</param>
    public void UpdateAP(Fighter fighter, int apCost)
    {
        if (fighter is Ally || fighter is Player)
        {
            DamageAllyAP(Party.IndexOf(fighter as Ally), apCost);
        }
        else
        {
            fighter.AP -= apCost;
        }
    }

    public void ResetAP()
    {
        foreach (Fighter fighter in TurnQueue)
        {
            UpdateAP(fighter, fighter.AP - fighter.MaxAP);
        }
    }

    /// <summary>
    /// Use the Battle scene's timer to wait for some amount of ms.
    /// </summary>
    /// <param name="ms">The amount of time to wait.</param>
    public async Task Wait(int ms)
    {
        var treeTimer = GetTree().CreateTimer(ms / 1000.0);
        await ToSignal(treeTimer, SceneTreeTimer.SignalName.Timeout);
    }
}
