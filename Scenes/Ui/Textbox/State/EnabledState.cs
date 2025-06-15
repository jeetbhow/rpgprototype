using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class EnabledState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public RichTextLabel NameLabel;
    [Export] public RichTextLabel TextLabel;
    [Export] public TextureRect Portrait;
    [Export] public VBoxContainer Buttons;

    [Export] public Timer TextAdvanceTimer;
    [Export] public Timer SfxTimer;
    [Export] public AudioStreamPlayer Sfx;


    // We store the sizes of the UI elements so that we can restore them later.
    private Vector2 nameLabelSize;
    private Vector2 textLabelSize;
    private Vector2 portraitSize;

    private readonly Dictionary<string, Texture2D> _portraits = new()
    {
        { "Jeet", (Texture2D)GD.Load("res://assets/characters/jeet/jeet-face.png") }
    };

    public override void _Ready()
    {
        // Store sizes
        nameLabelSize = NameLabel.CustomMinimumSize;
        textLabelSize = TextLabel.CustomMinimumSize;
        portraitSize = Portrait.CustomMinimumSize;

        TextAdvanceTimer.Timeout += OnTextAdvanceTimerTimeout;
        SfxTimer.Timeout += OnSfxTimerTimeout;
    }

    public override void Enter()
    {
        ResetTextboxState();
        LoadNextNode();
        StartTimers();
    }

    public void ResetTextboxState()
    {
        TextLabel.VisibleCharacters = 0;
        NameLabel.CustomMinimumSize = nameLabelSize;
        TextLabel.CustomMinimumSize = textLabelSize;
        Portrait.CustomMinimumSize = portraitSize;
    }

    private void LoadNextNode()
    {
        var node = Textbox.CurrNode;
        TextLabel.Text = node.Text;

        if (node.Name != null)
        {
            NameLabel.Text = node.Name;
        }
        else
        {
            NameLabel.CustomMinimumSize = Vector2.Zero;
        }
        if (node.Portrait != null && _portraits.TryGetValue(node.Portrait, out var tex))
        {
            Portrait.Texture = tex;
        }
        else
        {
            TextLabel.CustomMinimumSize += Portrait.CustomMinimumSize;
            Portrait.CustomMinimumSize = Vector2.Zero;
        }
    }

    private void StartTimers()
    {
        TextAdvanceTimer.Start();
        SfxTimer.Start();
    }

    public override void _Input(InputEvent @event)
    {
        GetViewport().SetInputAsHandled();
        if (@event.IsActionPressed("Accept"))
        {
            TextLabel.VisibleRatio = 1.0f;
            EnterWaitingState();
        }
    }

    private void EnterWaitingState()
    {
        SfxTimer.Stop();
        TextAdvanceTimer.Stop();

        if (Textbox.CurrNode is ChoiceNode)
        {
            EmitSignal(SignalName.StateUpdate, "ChoiceState");
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, "WaitingState");
        }
    }

    private void OnTextAdvanceTimerTimeout()
    {
        if (TextLabel.VisibleRatio < 1.0f)
        {
            TextLabel.VisibleCharacters += 1;
        }
        else
        {
            EnterWaitingState();
        }
    }

    private void OnSfxTimerTimeout()
    {
        Sfx.Play();
    }
}
