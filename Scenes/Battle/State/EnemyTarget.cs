using Godot;
using System;
using System.Threading.Tasks;

using Signal;
using Combat.Actors;
using Combat.UI;

namespace Combat;

public partial class EnemyTarget : StateNode
{
    [Export] public PackedScene ChoiceContentScene { get; set; }
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode PlayerTurn { get; set; }
    [Export] public StateNode TalkMenu { get; set; }

    private int _selectedEnemyIndex;
    private bool _AttackInProgress;

    private EnemyNode _hoveredEnemySprite;

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
                SelectEnemy(1);
                break;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                SelectEnemy(-1);
                break;
            case InputEventKey k when k.IsActionPressed("Accept") && Battle.CurrPlayerState == Battle.PlayerState.Attacking:
                if (!_AttackInProgress && player.AP >= player.Weapon.APCost)
                {
                    await Attack(player);
                }
                break;
            case InputEventKey k when k.IsActionPressed("Accept") && Battle.CurrPlayerState == Battle.PlayerState.Talking:
                Battle.FighterTargetedByPlayer = Battle.Enemies[_selectedEnemyIndex];
                EmitSignal(SignalName.StateUpdate, TalkMenu.Name);
                break;
            case InputEventKey k when k.IsActionPressed("Cancel"):
                SignalHub.Instance.EmitSignal(SignalHub.SignalName.AttackCancelled);
                EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
                break;
        }
    }

    public void SelectEnemy(int delta)
    {
        int count = Battle.Enemies.Count;
        _selectedEnemyIndex = (_selectedEnemyIndex + delta + count) % count;
        SignalHub.Instance.EmitSignal(SignalHub.SignalName.EnemySelected, Battle.Enemies[_selectedEnemyIndex], _selectedEnemyIndex);
    }

    public async Task Attack(Player player)
    {
        _AttackInProgress = true;

        var attackFinished = SignalHub.WaitForEventAsync<FighterEventArgs>(
            h => SignalHub.FighterAttacked += h,
            h => SignalHub.FighterAttacked -= h
        );

        Enemy enemy = Battle.Enemies[_selectedEnemyIndex];

        await Battle.UI.Log.AppendLine($"{player.Name} attacks {enemy.Name}.");
        await Game.Instance.Wait(200);

        SignalHub.RaiseAttackRequested(
            player,
            enemy,
            player.Weapon
        );

        await attackFinished;
        await Battle.UI.Log.AppendLine($"{enemy.Name} takes {player.Weapon.Damage} damage.");

        EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
    }

    public override async Task Enter()
    {
        if (Battle.CurrFighter is not Player)
        {
            throw new InvalidOperationException(
                $"{Name}: Can only be entered by a Player." +
                $"Current fighter: {Battle.CurrFighter.GetType().Name}"
            );
        }

        _selectedEnemyIndex = 0;
        _AttackInProgress = false;

        switch (Battle.CurrPlayerState)
        {
            case Battle.PlayerState.Attacking:
                SetupAttackTargetMenu();
                break;
            case Battle.PlayerState.Talking:
                SetupTalkTargetMenu();
                break;
        }

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

    public void SetupAttackTargetMenu()
    {
        Player player = Battle.CurrFighter as Player;
        string weapon = player.Weapon.Name;
        int apCost = player.Weapon.APCost;

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
    }

    public void SetupTalkTargetMenu()
    {
        Battle.UI.Commands.Choices.RemoveAll();
        foreach (var enemy in Battle.Enemies)
        {
            ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
            choice.Label.Text = enemy.Name;
            Battle.UI.Commands.Choices.AddChoice(choice);
        }

        SignalHub.Instance.EmitSignal(
            SignalHub.SignalName.EnemySelected,
            Battle.Enemies[_selectedEnemyIndex],
            _selectedEnemyIndex
        );
    }
}
