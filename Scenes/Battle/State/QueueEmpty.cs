using Godot;
using System.Threading.Tasks;

namespace Combat;

public partial class QueueEmpty : StateNode
{
    [Export]
    Battle Battle;
   
    [Export]
    StateNode TurnStart;

    public override async Task Enter()
    {
        await Battle.DetermineTurnOrder();
        EmitSignal(SignalName.StateUpdate, TurnStart.Name);
    }
}
