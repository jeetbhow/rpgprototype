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
         _AttackInProgress ||
         Battle.CurrFighter is not Player player)
        {
            return;
        }

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
                if (!_AttackInProgress && player.AP >= player.Weapon.Ability.APCost)
                {
                    await Attack(player);
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

        await Battle.UI.Log.AppendLine($"{player.Name} attacks {enemy.Name}.");
        await Battle.Wait(500);

        SignalHub.Instance.EmitSignal(
            SignalHub.SignalName.AttackRequested,
            player,
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
        int apCost = player.Weapon.Ability.APCost;

        Battle.UI.Commands.Choices.Clear();
        foreach (var enemy in Battle.Enemies)
        {
            ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
            choice.Label.Text = enemy.Name + $" [color={Game.APColor.ToHtml()}](AP: {apCost})[/color]";
            choice.Enabled = player.AP >= apCost;
            Battle.UI.Commands.Choices.AddChoice(choice);
        }

        SignalHub.Instance.EmitSignal(
            SignalHub.SignalName.EnemySelected,
            Battle.Enemies[_selectedEnemyIndex],
            _selectedEnemyIndex
        );
    }
}
