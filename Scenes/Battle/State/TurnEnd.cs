using Godot;
using System.Threading.Tasks;

namespace Combat;

public partial class TurnEnd : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode TurnQueueEmpty { get; set; }
    [Export] public StateNode TurnStart { get; set; }

    public override async Task Enter()
    {
        Battle.UI.AdvanceTurnQueue();
        Battle.UI.Commands.Hide();
        
        Fighter fighter = Battle.TurnQueue.Dequeue();
        fighter.AP = fighter.MaxAP;

        if (Battle.TurnQueue.Count > 0)
        {
            EmitSignal(SignalName.StateUpdate, TurnStart.Name);
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, TurnQueueEmpty.Name);
        }

        await Task.CompletedTask;
    }
}
