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
    public List<Fighter> Party { get; private set; } = [];
    public List<Fighter> Enemies { get; private set; } = [];
    public Queue<Fighter> TurnQueue { get; private set; } = new();
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

            var newFighter = new Fighter(
                ally.Name,
                Fighter.FighterType.Ally,
                ally.HP,
                ally.AP,
                ally.Strength,
                ally.Endurance,
                ally.Athletics
            );

            if (ally is Player player)
            {
                newFighter.Type = Fighter.FighterType.Player;
            }

            Party.Add(newFighter);

            PartyInfoPanel panel = PartyInfoPanelScene.Instantiate<PartyInfoPanel>();
            UI.PartyInfoHBox.AddChild(panel);

            panel.PartyMemberName = ally.Name;
            panel.HP = ally.HP;
            panel.MaxHP = ally.MaxHP;
            panel.AP = ally.AP;
            panel.MaxAP = ally.MaxAP;
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
            if (enemy is BattleEnemy e)
            {
                GD.Print($"Adding enemy: {e.Data.Name}");
                Enemies.Add(new Fighter(
                     e.Data.Name,
                     Fighter.FighterType.Enemy,
                     e.Data.HP,
                     e.Data.AP,
                     e.Data.Strength,
                     e.Data.Endurance,
                     e.Data.Athletics
                 ));

                await e.FadeIn();

                string introduction = e.Data.Introduction;
                await UI.Log.AppendLine(introduction);
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
    public void DetermineTurnOrder()
    {
        Fighter[] allParticipants = new Fighter[Party.Count + Enemies.Count];
        Party.CopyTo(allParticipants, 0);
        Enemies.CopyTo(allParticipants, Party.Count);

        foreach (var participant in allParticipants)
        {
            RandomNumberGenerator rng = new();
            int d1 = rng.RandiRange(_D6Min, _D6Max);
            int d2 = rng.RandiRange(_D6Min, _D6Max);

            participant.Initiative = d1 + d2 + participant.Athletics;
        }

        TurnQueue = new Queue<Fighter>(allParticipants.OrderByDescending(p => p.Initiative));
    }

    /// <summary>
    /// Highlights the enemy at the specified index in the battle.
    /// </summary>
    /// <param name="index">The index of the enemy to highlight.</param>
    public void HighlightEnemy(int index)
    {
        var battleEnemy = EnemyNodes.GetChild<BattleEnemy>(index, false);
        battleEnemy?.Highlight();
    }

    /// <summary>
    /// Stops the animation of the enemy at the specified index.
    /// </summary>
    /// <param name="index"></param>
    public void UnhighlightEnemy(int index)
    {
        var battleEnemy = EnemyNodes.GetChild<BattleEnemy>(index, false);
        battleEnemy?.StopAnimation();
    }

    /// <summary>
    /// Damages the enemy at the specified index by the given amount.
    /// </summary>
    /// <param name="index">The index of the enemy to damage.</param>
    /// <param name="damage">The amount of damage to deal.</param>
    public void DamageEnemy(int index, int damage)
    {
        var battleEnemy = EnemyNodes.GetChild<BattleEnemy>(index, false);
        battleEnemy?.TakeDamage(damage);
    }

    /// <summary>
    /// Updates the Action Points (AP) of the specified fighter by subtracting the given AP cost.
    /// </summary>
    /// <param name="fighter"></param>
    /// <param name="apCost"></param>
    public void UpdateAP(Fighter fighter, int apCost)
    {
        fighter.AP -= apCost;
        if (fighter.Type == Fighter.FighterType.Ally ||
            fighter.Type == Fighter.FighterType.Player)
        {
            GD.Print("Updating AP for ally: " + fighter.Name);
            PartyInfoPanel panel = UI.PartyInfoHBox.GetChild<PartyInfoPanel>(Party.IndexOf(fighter), false);
            panel.AP = fighter.AP;
        }
    }
}
