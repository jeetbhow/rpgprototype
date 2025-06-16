using Godot;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class EnabledState : StateNode
{
    [Export] public Textbox Textbox { get; set; }
    [Export] public StateNode ChoiceStateNode { get; set; }
    [Export] public StateNode WaitingStateNode { get; set; }

    private bool _skipRequested;
    private string _fullText;

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed("Accept")) return;

        _skipRequested = true;
        Textbox.TextLabel.Text = _fullText;
        Textbox.SetVisibleChars(_fullText.Length);        
        GetViewport().SetInputAsHandled();
    }

    public override async Task Enter()
    {
        _skipRequested = false;
        Textbox.TextLabel.Text = "";
        Textbox.ResetTextboxState();
        await RenderLine(Textbox.LoadNextNode());
    }

    public override async Task Exit()
    {
        _skipRequested = false;
        _fullText = "";
    }
    

    public async Task RenderLine(string line)
    {
        string pattern = @"(\[\w+=\d+\])";
        _fullText = Regex.Replace(line, pattern, "");
        string[] parts = Regex.Split(line, pattern);
        int visibleChars = 0;

        foreach (var part in parts)
        {
            if (_skipRequested) break;

            // Parse the command
            if (part.StartsWith('[') && part.EndsWith(']'))
            {
                string inner = part[1..^1];
                string[] tokens = inner.Split('=');
                string command = tokens[0];
                string value = tokens[1];

                switch (command)
                {
                    case "pause":
                        await ToSignal(
                            GetTree().CreateTimer(float.Parse(value)),
                            Timer.SignalName.Timeout
                        );
                        break;
                }
                continue;
            }

            // Render the actual text with the scrolling effect.
            int start = visibleChars;
            int end = start + part.Length;
            Textbox.TextLabel.Text += part;

            await Typewriter(start, end);

            visibleChars = end;
        }

        Textbox.SfxTimer.Stop();
        UpdateState();
    }

    public async Task Typewriter(int start, int end)
    {
        Textbox.SfxTimer.Start();

        int curr = start;
        while (curr < end)
        {
            Textbox.SetVisibleChars(++curr);
            await ToSignal(GetTree().CreateTimer(0.02f), Timer.SignalName.Timeout);
            if (_skipRequested) return;
        }

        Textbox.SfxTimer.Stop();
    }

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
