using Godot;
using System.Threading.Tasks;

namespace Combat;

public partial class PlayerTurn : StateNode
{
    private int _index;
    private bool _isPlayerTurn = false;

    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattlePlayerAttackMenu { get; set; }
    [Export] public StateNode TurnEnd { get; set; }

    public override async void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.IsPressed())
        {
            return;
        }

        int prevIndex = _index;
        int numCmds = Battle.UI.Commands.GetChoices().Length;

        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                _index = (_index + 1) % numCmds;
                break;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                _index = (_index - 1 + numCmds) % numCmds;
                break;
            case InputEventKey k when k.IsActionPressed("Accept"):
                ChoiceContent choice = Battle.UI.Commands.GetChoices()[_index];
                switch (choice.Label.Text)
                {
                    case "Attack":
                        EmitSignal(SignalName.StateUpdate, BattlePlayerAttackMenu.Name);
                        break;
                    case "Defend":
                        GD.Print("Defend chosen");
                        break;
                    case "Talk":
                        GD.Print("Talk chosen");
                        break;
                    case "Item":
                        GD.Print("Item chosen");
                        break;
                    case "Run":
                        GD.Print("Run chosen");
                        break;
                    case "End Turn":
                        _isPlayerTurn = false;
                        EmitSignal(SignalName.StateUpdate, TurnEnd.Name);
                        break;
                }
                break;
        }

        if (prevIndex != _index)
        {
            Battle.UI.Commands.Choices.HideArrow(prevIndex);
            Battle.UI.Commands.Choices.ShowArrow(_index);
        }
    }

    public override async Task Enter()
    {
        _index = 0;

        if (!_isPlayerTurn)
        {
            await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} is ready to fight!");
            _isPlayerTurn = true;
            Battle.UI.Commands.Visible = true;
        }

        Battle.UI.Commands.TextLabel.Text = $"";
        Battle.UI.Commands.TextLabel.Visible = false;
        Battle.UI.ShowPlayerCommands(_index);
    }
}
