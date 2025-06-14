using Godot;

public partial class Waiting : StateNode
{
    [Export]
    public Textbox Textbox;

    [Export]
    public RichTextLabel NameLabel;

    [Export]
    public RichTextLabel TextLabel;

    [Export]
    public TextureRect Portrait;

    public override void _Input(InputEvent @event)
    {
        GetViewport().SetInputAsHandled();

        if (@event.IsActionPressed("Accept"))
        {
            Textbox.CurrNode = Textbox.CurrNode.Next;

            if (Textbox.CurrNode != null)
            {
                TextLabel.VisibleCharacters = 0;
                EmitSignal(SignalName.StateUpdate, "Enabled");
            }
            else
            {
                EmitSignal(SignalName.StateUpdate, "Disabled");
            }
        }
    }
}
