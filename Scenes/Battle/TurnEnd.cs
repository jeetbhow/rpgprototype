using Godot;
using System.Threading.Tasks;

public partial class TurnEnd : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode TurnQueueEmpty { get; set; }
    [Export] public StateNode TurnStart { get; set; }

    public override async Task Enter()
    {
        Battle.UI.TQPopFront();
        Battle.UI.Commands.Hide();
        Battle.TurnQueue.Dequeue();
        if (Battle.TurnQueue.Count > 0)
        {
            EmitSignal(SignalName.StateUpdate, TurnStart.Name);
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, TurnQueueEmpty.Name);
        }
    }
}
