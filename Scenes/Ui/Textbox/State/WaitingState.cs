using Godot;

public partial class WaitingState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledStateNode;
    [Export] public StateNode DisabledStateNode;

    public override void _Input(InputEvent @event)
    {
        GetViewport().SetInputAsHandled();

        if (@event.IsActionPressed("Accept"))
        {
            Textbox.CurrNode = Textbox.CurrNode.Next;
            if (Textbox.CurrNode == null)
            {
                EmitSignal(SignalName.StateUpdate, DisabledStateNode.Name);
            }
            else
            {
                EmitSignal(SignalName.StateUpdate, EnabledStateNode.Name);
            }
        }
    }
}
