using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerTurn : StateNode
{
    private int _index;
    private bool _isPlayerTurn = false;

    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattlePlayerAttackMenu { get; set; }
    [Export] public StateNode BattleNPCTurn { get; set; }
    [Export] public StateNode TurnQueueEmpty { get; set; }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.IsPressed())
        {
            return;
        }

        int prevIndex = _index;
        int numCmds = Battle.UI.CommandTextbox.GetChoices().Length;

        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                _index = (_index + 1) % numCmds;
                break;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                _index = (_index - 1 + numCmds) % numCmds;
                break;
            case InputEventKey k when k.IsActionPressed("Accept"):
                ChoiceContent choice = Battle.UI.CommandTextbox.GetChoices()[_index];
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

                        if (Battle.TurnQueue.Count > 0)
                        {
                            Battle.CurrFighter = Battle.TurnQueue.Dequeue();
                            if (Battle.CurrFighter is not Player)
                                EmitSignal(SignalName.StateUpdate, BattleNPCTurn.Name);
                            else
                                throw new InvalidOperationException("Player can't be in the turn queue after their turn ends.");
                        }
                        else
                        {
                            EmitSignal(SignalName.StateUpdate, TurnQueueEmpty.Name);
                        }
                        break;
                }
                break;
        }

        if (prevIndex != _index)
        {
            Battle.UI.CommandTextbox.Choices.HideArrow(prevIndex);
            Battle.UI.CommandTextbox.Choices.ShowArrow(_index);
        }
    }

    public override async Task Enter()
    {
        _index = 0;

        foreach (Enemy enemy in Battle.Enemies)
        {
            if (!enemy.IsAlive)
            {
                await Battle.UI.Log.AppendLine($"{enemy.Name} has fallen!");
            }
        }

        if (!_isPlayerTurn)
        {
            await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} is ready to fight!");
            _isPlayerTurn = true;
        }

        Battle.UI.ShowPlayerCommands(_index);
    }
}
