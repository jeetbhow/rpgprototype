using Godot;
using System.Threading.Tasks;

public partial class EnabledState : StateNode
{
    [Export] public UI UI { get; set; }
    [Export] public StateNode ChoiceStateNode { get; set; }
    [Export] public StateNode WaitingStateNode { get; set; }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("Accept") || UI.Textbox.SkipRequested) return;
            UI.Skip();
        GetViewport().SetInputAsHandled();
    }

    public override async Task Enter()
    {
        await UI.WriteText();
        if (UI.CurrNode is ChoiceNode)
        {
            EmitSignal(SignalName.StateUpdate, ChoiceStateNode.Name);
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, WaitingStateNode.Name);
        }
    }

    public override async Task Exit()
    { /* Do nothing */ }

}
