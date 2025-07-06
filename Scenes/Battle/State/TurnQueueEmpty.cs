using Godot;
using System.Threading.Tasks;

public partial class TurnQueueEmpty : StateNode
{
    [Export] Battle Battle;
    [Export] StateNode TurnStart;

    public override async Task Enter()
    {
        await Battle.DetermineTurnOrder();
        Battle.ResetAP();
        EmitSignal(SignalName.StateUpdate, TurnStart.Name);
    }
}
