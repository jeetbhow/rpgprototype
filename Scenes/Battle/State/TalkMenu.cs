using Godot;
using System.Threading.Tasks;

using Combat.Actors;
using Combat.Talk;
using System;
using Combat.UI;

public partial class TalkMenu : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public PackedScene ChoiceContentScene { get; set; }
    [Export] public StateNode PlayerTurn { get; set; }
    [Export] public StateNode EnemyTarget { get; set; }
    [Export] public StateNode Barter { get; set; }

    private const int _TalkAPCost = 2;

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
            ChoiceContent choiceContent = Battle.UI.Commands.Choices.GetSelectedChoice();
            if (choiceContent.Enabled)
            {
                HandleTalkAction();
            }
        }
    }

    public async void HandleTalkAction()
    {
        _isCurrentlyTalking = true;
        Battle.UI.Commands.Choices.Active = false;

        int index = Battle.UI.Commands.Choices.SelectedIndex;
        TalkAction action = _targetedEnemy.TalkActions[index];
        EnemyNode enemyNode = Battle.EnemyNodes.Find(node => node.EnemyData == _targetedEnemy);

        // TODO: Find a better formula for handing AP costs for speech checks.
        Game.Instance.Player.AP -= _TalkAPCost;
        TalkActionEffect effect = await enemyNode.RespondToTalkAction(player: Game.Instance.Player, action);

        if (effect != TalkActionEffect.Barter)
        {
            EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, Barter.Name);
        }
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
            ChoiceContent choiceContent = ChoiceContentScene.Instantiate<ChoiceContent>();
            choiceContent.Label.Text = action.Text;
            choiceContent.Enabled =
                Game.Instance.Player.AP >= _TalkAPCost &&
                action.IsEnabled(Game.Instance.Player, _targetedEnemy);
            choiceContent.Visible = action.IsVisible(Game.Instance.Player, _targetedEnemy);
            
            Battle.UI.Commands.Choices.AddChoice(choiceContent);
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
