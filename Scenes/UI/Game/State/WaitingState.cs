using System.Threading.Tasks;
using Godot;

public partial class WaitingState : StateNode
{
    [Export] public UI UI;
    [Export] public StateNode EnabledStateNode;
    [Export] public StateNode DisabledStateNode;

    public override async Task Enter()
    {}
    public override async Task Exit() { }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Accept"))
        {
            UI.CurrNode = UI.CurrNode.Next;
            if (UI.CurrNode == null)
            {
                EmitSignal(SignalName.StateUpdate, DisabledStateNode.Name);
            }
            else
            {
                EmitSignal(SignalName.StateUpdate, EnabledStateNode.Name);
            }
            GetViewport().SetInputAsHandled();
        }
    }
}
