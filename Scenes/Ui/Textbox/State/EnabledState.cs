using Godot;

public partial class EnabledState : StateNode
{
    [Export] public Textbox Textbox { get; set; }
    [Export] public StateNode ChoiceStateNode { get; set; }
    [Export] public StateNode WaitingStateNode { get; set; }

    public override void _Input(InputEvent @event)
    {
        GetViewport().SetInputAsHandled();
        if (@event.IsActionPressed("Accept"))
        {
            Textbox.TextLabel.VisibleRatio = 1.0f;
            UpdateState();
        }
    }

    public override void _Ready()
    {
        Textbox.TextAdvanceTimer.Timeout += OnTextAdvanceTimerTimeout;
        Textbox.SfxTimer.Timeout += OnSfxTimerTimeout;
    }

    public override void Enter()
    {
        Textbox.ResetTextboxState();
        Textbox.LoadNextNode();
        Textbox.StartTimers();
    }

    private void OnTextAdvanceTimerTimeout()
    {
        if (Textbox.TextLabel.VisibleRatio < 1.0f)
        {
            Textbox.TextLabel.VisibleCharacters += 1;
        }
        else
        {
            UpdateState();
        }
    }

    private void OnSfxTimerTimeout()
    {
        Textbox.Sfx.Play();
    }
    
     private void UpdateState()
    {
        Textbox.SfxTimer.Stop();
        Textbox.TextAdvanceTimer.Stop();

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
