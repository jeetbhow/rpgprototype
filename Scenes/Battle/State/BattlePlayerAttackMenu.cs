using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class BattlePlayerAttackMenu : StateNode
{
    private const int DefaultDmg = 2;
    private const int DefaultApCost = 2;
    private const int HitDelayMs = 500;

    private int _selectedEnemyIndex;
    private bool _AttackInProgress;

    private EnemyBattleSprite _hoveredEnemySprite;

    [Export] public PackedScene ChoiceContentScene { get; set; }
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattlePlayerTurn { get; set; }

    public override async void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.IsPressed() || _AttackInProgress)
        {
            return;
        }

        int prevIndex = _selectedEnemyIndex;
        int numChoices = Battle.UI.Commands.Choices.GetChildCount();
        int prevApCost = GetAPCost(Battle.CurrFighter, Battle.Enemies[prevIndex]);
        int apCost = GetAPCost(Battle.CurrFighter, Battle.Enemies[_selectedEnemyIndex]);
        int pnlIndex = Battle.Party.IndexOf((Ally)Battle.CurrFighter);
        PartyInfoPanel panel = Battle.UI.GetPartyInfoPanel(pnlIndex);

        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                MoveSelection(1);
                break;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                MoveSelection(-1);
                break;
            case InputEventKey k when k.IsActionPressed("Accept"):
                if (!_AttackInProgress && Battle.CurrFighter.AP >= apCost)
                {
                    await Attack();
                    EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                }
                break;
            case InputEventKey k when k.IsActionPressed("Cancel"):
                _hoveredEnemySprite.HideHP();
                EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                break;
        }

        if (prevIndex != _selectedEnemyIndex && Battle.CurrFighter is not Player)
        {

        }
    }

    public void MoveSelection(int delta)
    {
        int count = Battle.Enemies.Count;
        int prev = _selectedEnemyIndex;
        _selectedEnemyIndex = (_selectedEnemyIndex + delta + count) % count;
    }

    public async Task Attack()
    {
        _AttackInProgress = true;

        Enemy enemy = Battle.Enemies[_selectedEnemyIndex];
        EnemyBattleSprite sprite = Battle.GetEnemySprite(_selectedEnemyIndex);

        await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} attacks {enemy.Name}.");
        await Battle.Wait(500);

        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Slash);
        await sprite.PlayEffect("slash");
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 8.0f);
        sprite.Bleed();
        sprite.Flinch();
        Battle.UI.Log.AppendLine($"{enemy.Name} takes {DefaultDmg} damage.");
        await sprite.TakeDamage(DefaultDmg);
        Battle.UpdateAP(Battle.CurrFighter, GetAPCost(Battle.CurrFighter, enemy));
        _hoveredEnemySprite.HideHP();
        sprite.StopAnimation();

        enemy.HP -= DefaultDmg;

        if (enemy.HP <= 0)
        {
            await sprite.Die();
            sprite.QueueFree();

            Battle.Enemies.RemoveAt(_selectedEnemyIndex);
            Battle.TurnQueue = new Queue<Fighter>(Battle.TurnQueue.Where(fighter => fighter != enemy));
            Battle.UI.SetTurnQueue(Battle.TurnQueue);

            await Battle.UI.Log.AppendLine(enemy.DeathMsgLog);
        }
    }

    public override async Task Enter()
    {
        _selectedEnemyIndex = 0;
        _AttackInProgress = false;

        Battle.UI.Commands.Choices.Clear();

        foreach (var enemy in Battle.Enemies)
        {
            int apCost = GetAPCost(Battle.CurrFighter, enemy);
            ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
            choice.Label.Text = enemy.Name + $" [color={Game.APColor.ToHtml()}](AP: {apCost})[/color]";
            choice.Enabled = Battle.CurrFighter.AP >= apCost;
            Battle.UI.Commands.Choices.AddChoice(choice);
        }

        Battle.UI.Commands.Choices.ShowArrow(_selectedEnemyIndex);

        _hoveredEnemySprite = Battle.GetEnemySprite(_selectedEnemyIndex);
        _hoveredEnemySprite.ShowHP();
    }
    
    
    /// <summary>
    /// Get the AP cost for an attack by an attacker against a defender.
    /// </summary>
    /// <param name="attacker">The fighter that's attacking.</param>
    /// <param name="defender">The fighter that's defending.</param>
    /// <returns>The amount of AP used for the attack.</returns>
    public int GetAPCost(Fighter attacker, Fighter defender)
    {
        // For now, I'll use a fixed AP cost for attacks.
        // In the future, this could depend on the attack type, weapon, enemy, etc
        int baseCost = 2;
        return baseCost;
    }
}
