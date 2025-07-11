using Godot;
using System;
using System.Threading.Tasks;

namespace Combat;

public partial class AttackMenu : StateNode
{
    [Export]
    public PackedScene ChoiceContentScene { get; set; }

    [Export]
    public Battle Battle { get; set; }

    [Export]
    public StateNode BattlePlayerTurn { get; set; }

    private const int DefaultDmg = 2;
    private int _selectedEnemyIndex;
    private bool _AttackInProgress;

    private EnemyBattleSprite _hoveredEnemySprite;

    public override async void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent ||
         !keyEvent.IsPressed() ||
         _AttackInProgress)
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
                    await Attack((Player)Battle.CurrFighter);
                }
                break;
            case InputEventKey k when k.IsActionPressed("Cancel"):
                SignalHub.Instance.EmitSignal(SignalHub.SignalName.AttackCancelled);
                EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                break;
        }
    }

    public void MoveSelection(int delta)
    {
        int count = Battle.Enemies.Count;
        _selectedEnemyIndex = (_selectedEnemyIndex + delta + count) % count;
        SignalHub.Instance.EmitSignal(SignalHub.SignalName.EnemySelected, Battle.Enemies[_selectedEnemyIndex], _selectedEnemyIndex);
    }

    public async Task Attack(Player player)
    {
        _AttackInProgress = true;

        Enemy enemy = Battle.Enemies[_selectedEnemyIndex];

        await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} attacks {enemy.Name}.");
        await Battle.Wait(500);

        SignalHub.Instance.EmitSignal(
            SignalHub.SignalName.AttackRequested,
             Battle.CurrFighter,
            enemy,
            player.Weapon.Ability
        );

        await Battle.Wait(500);

        await Battle.UI.Log.AppendLine($"{enemy.Name} takes {DefaultDmg} damage.");
        EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
    }

    public override async Task Enter()
    {
        if (Battle.CurrFighter is not Player player)
        {
            throw new InvalidOperationException(
                "AttackMenu can only be entered by a Player fighter. " +
                $"Current fighter: {Battle.CurrFighter.GetType().Name}"
            );
        }

        _selectedEnemyIndex = 0;
        _AttackInProgress = false;

        Battle.UI.Commands.Choices.Clear();

        foreach (var enemy in Battle.Enemies)
        {
            int apCost = GetAPCost(player, enemy);
            ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
            choice.Label.Text = enemy.Name + $" [color={Game.APColor.ToHtml()}](AP: {apCost})[/color]";
            choice.Enabled = player.AP >= apCost;
            Battle.UI.Commands.Choices.AddChoice(choice);
        }

        _hoveredEnemySprite = Battle.GetEnemySprite(_selectedEnemyIndex);
        SignalHub.Instance.EmitSignal(SignalHub.SignalName.EnemySelected, Battle.Enemies[_selectedEnemyIndex], _selectedEnemyIndex);
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
        int baseCost = 1;
        return baseCost;
    }
}
