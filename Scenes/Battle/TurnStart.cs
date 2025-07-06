using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class TurnStart : StateNode
{
    [Export] Battle Battle { get; set; }
    [Export] StateNode NPCTurn { get; set; }
    [Export] StateNode PlayerTurn { get; set; }

    public override async Task Enter()
    {
        Battle.CurrFighter = Battle.TurnQueue.Peek();
        TurnQueuePanel p = Battle.UI.TQPeek();
        p.Glow();

        // Emit a signal to go to the state node corresponding to either the Player or NPC turn.
        switch (Battle.CurrFighter)
        {
            case Enemy:
                EmitSignal(SignalName.StateUpdate, NPCTurn.Name);
                break;
            case Player:
                EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
                break;
            case Ally:
                EmitSignal(SignalName.StateUpdate, NPCTurn.Name);
                break;
        }
    }
}
