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

        var attackFinished = ToSignal(
            SignalHub.Instance,
            SignalHub.SignalName.FighterAttacked);

        Enemy enemy = Battle.Enemies[_selectedEnemyIndex];

        await Battle.UI.Log.AppendLine($"{player.Name} attacks {enemy.Name}.");
        await Game.Instance.Wait(200);

        SignalHub.Instance.EmitSignal(
            SignalHub.SignalName.AttackRequested,
            player,
            enemy,
            player.Weapon.Ability
        );

        await attackFinished;
        await Battle.UI.Log.AppendLine($"{enemy.Name} takes {player.Weapon.Ability.Damage} damage.");

        EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
    }

    public override async Task Enter()
    {
        if (Battle.CurrFighter is not Player player)
        {
            throw new InvalidOperationException(
                "AttackMenu can only be entered by a Player." +
                $"Current fighter: {Battle.CurrFighter.GetType().Name}"
            );
        }

        _selectedEnemyIndex = 0;
        _AttackInProgress = false;

        string weapon = player.Weapon.Name;
        int apCost = player.Weapon.Ability.APCost;

        Battle.UI.Commands.TextLabel.Text = $"Weapon: {weapon}";
        Battle.UI.Commands.TextLabel.Visible = true;

        Battle.UI.Commands.Choices.RemoveAll();
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

        await Task.CompletedTask;
    }

    public override async Task Exit()
    {
        Battle.UI.Commands.TextLabel.Text = null;
        Battle.UI.Commands.TextLabel.Visible = false;
        Battle.UI.Commands.Choices.Active = false;
        Battle.UI.Commands.Choices.RemoveAll();
        await Task.CompletedTask;
    }
}
