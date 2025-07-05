using Godot;
using System.Threading.Tasks;

public partial class TurnQueueEmpty : StateNode
{
    [Export] Battle Battle;
    [Export] StateNode PlayerTurn;
    [Export] StateNode NPCTurn;

    public override async Task Enter()
    {
        await Battle.DetermineTurnOrder();
        Battle.ResetAP();

        Battle.CurrFighter = Battle.TurnQueue.Dequeue();
        switch (Battle.CurrFighter)
        {
            case Player:
                EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
                break;
            case Ally:
            case Enemy:
                EmitSignal(SignalName.StateUpdate, NPCTurn.Name);
                break;
            default:
                GD.PrintErr("Unknown fighter type in TurnQueueEmpty state.");
                break;
        }
    }
}
