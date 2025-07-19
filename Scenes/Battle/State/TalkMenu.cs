using Godot;
using System.Threading.Tasks;

using Combat.Actors;
using Combat.Talk;
using System;
using Combat.UI;

public partial class TalkMenu : StateNode
{
    [Export]
    public Battle Battle { get; set; }
    [Export]
    public StateNode PlayerTurn { get; set; }
    [Export]
    public StateNode EnemyTarget { get; set; }

    private Enemy _targetedEnemy;
    private bool _isCurrentlyTalking = false;

    public override void _Input(InputEvent @event)
    {
        if (_isCurrentlyTalking)
        {
            return;
        }

        if (@event.IsActionPressed("Cancel"))
        {
            EmitSignal(SignalName.StateUpdate, EnemyTarget.Name);
            return;
        }

        if (@event.IsActionPressed("Accept"))
        {
            HandleTalkAction();
        }
    }

    public async void HandleTalkAction()
    {
        _isCurrentlyTalking = true;

        Battle.UI.Commands.Choices.Active = false;
        int index = Battle.UI.Commands.Choices.SelectedIndex;
        TalkAction action = _targetedEnemy.TalkActions[index];
        EnemyNode enemyNode = Battle.EnemyNodes.Find(node => node.EnemyData == _targetedEnemy);
        await enemyNode.RespondToTalkAction(Game.Instance.Player, action);
        EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
    }

    public override async Task Enter()
    {
        if (Battle.FighterTargetedByPlayer is not Enemy enemy)
        {
            throw new InvalidOperationException("TalkMenu can only be entered when an enemy is targeted.");
        }

        _targetedEnemy = enemy;

        foreach (TalkAction action in enemy.TalkActions)
        {
            Battle.UI.Commands.Choices.AddChoice(action.Text);
        }
        Battle.UI.Commands.Choices.HideAllArrows();
        Battle.UI.Commands.Choices.ShowArrow(0);
        Battle.UI.Commands.Choices.Active = true;

        await Task.CompletedTask;
    }

    public override async Task Exit()
    {
        _isCurrentlyTalking = false;
        Battle.UI.Commands.Choices.RemoveAll();
        Battle.UI.Commands.Choices.Active = false;

        await Task.CompletedTask;
    }
}
