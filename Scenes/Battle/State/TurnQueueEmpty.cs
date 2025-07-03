using Godot;
using System.Threading.Tasks;

public partial class TurnQueueEmpty : StateNode
{
    [Export] Battle Battle;
    [Export] StateNode PlayerTurn;
    [Export] StateNode NPCTurn;

    public override async Task Enter()
    {
        Battle.DetermineTurnOrder();

        foreach (Fighter fighter in Battle.TurnQueue)
        {
            fighter.AP = fighter.MaxAP;
            // If the fighter is an Ally, then reset their panel in the UI as well.
            if (fighter is Ally)
            {
                Battle.UI.ResetAP();
            }
        }

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
