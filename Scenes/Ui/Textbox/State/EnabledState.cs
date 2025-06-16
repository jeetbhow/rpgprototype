using Godot;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class EnabledState : StateNode
{
    [Export] public Textbox Textbox { get; set; }
    [Export] public StateNode ChoiceStateNode { get; set; }
    [Export] public StateNode WaitingStateNode { get; set; }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("Accept")) return;

        Textbox.SkipRequested = true;
        Textbox.TextLabel.Text = Textbox.FullText;
        Textbox.SetVisibleChars(Textbox.FullText.Length);
        Textbox.SfxTimer.Stop();
             
        GetViewport().SetInputAsHandled();
    }

    public override async Task Enter()
    {
        Textbox.ResetTextboxState();
        await Textbox.ProcessAndWriteText(Textbox.LoadNextNode());
        UpdateState();
    }

    public override async Task Exit()
    { /* Do nothing */ }

    private void UpdateState()
    {
        if (Textbox.CurrNode is ChoiceNode)
        {
            EmitSignal(SignalName.StateUpdate, ChoiceStateNode.Name);
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, WaitingStateNode.Name);
        }
    }
}
