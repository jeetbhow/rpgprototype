using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat;

enum CommandType
{
    Invalid,
    Attack,
    Talk,
    Item,
    EndTurn,
}

public partial class PlayerTurn : StateNode
{
    [Export]
    public Battle Battle { get; set; }

    [Export]
    public StateNode EnemyTarget { get; set; }

    [Export]
    public StateNode EndTurn { get; set; }

    [Export]
    public StateNode ItemMenu { get; set; }

    private string[] CommandNames = ["Attack", "Talk", "Item", "End Turn"];

    private static readonly Dictionary<string, CommandType> Commands = new()
    {
        { "Attack", CommandType.Attack},
        { "Talk", CommandType.Talk },
        { "Item", CommandType.Item },
        { "End Turn", CommandType.EndTurn }
    };

    // The player's turn is still going if they have not yet explicitly
    // selected the end turn command. Depending on whether or not we're
    // entering this state for the first time, we may need to initialize
    // the commands list.
    private bool _turnIsStillGoing = false;

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("Accept") || !_turnIsStillGoing)
        {
            return;
        }

        string choice = Battle.UI.Commands.Choices.GetSelectedChoice().Label.Text;
        CommandType key = Commands.GetValueOrDefault(choice, CommandType.Invalid);
        switch (key)
        {
            case CommandType.Attack:
                Battle.CurrPlayerState = Battle.PlayerState.Attacking;
                EmitSignal(SignalName.StateUpdate, EnemyTarget.Name);
                break;
            case CommandType.Item:
                EmitSignal(SignalName.StateUpdate, ItemMenu.Name);
                break;
            case CommandType.Talk:
                Battle.CurrPlayerState = Battle.PlayerState.Talking;
                EmitSignal(SignalName.StateUpdate, EnemyTarget.Name);
                break;
            case CommandType.EndTurn:
                _turnIsStillGoing = false;
                EmitSignal(SignalName.StateUpdate, EndTurn.Name);
                break;
        }
    }

    public override async Task Enter()
    {
        if (!_turnIsStillGoing)
        {
            _turnIsStillGoing = true;

            await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} is ready to fight!");
            Battle.UI.Commands.Visible = true;
        }

        InitializeCommands();
    }

    public override async Task Exit()
    {
        DeactivateCommands();
        await Task.CompletedTask;
    }

    public void InitializeCommands()
    {
        Battle.UI.Commands.Choices.RemoveAll();
        foreach (var command in CommandNames)
        {
            Battle.UI.Commands.Choices.AddChoice(command);
        }
        Battle.UI.Commands.Choices.HideAllArrows();
        Battle.UI.Commands.Choices.ShowArrow(0);

        Battle.UI.Commands.Choices.Visible = true;
        Battle.UI.Commands.Choices.Active = true;
    }

    public void DeactivateCommands()
    {
        Battle.UI.Commands.Choices.RemoveAll();
        Battle.UI.Commands.Choices.Active = false;
    }
}
