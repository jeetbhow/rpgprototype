using Godot;
using Newtonsoft.Json;

public partial class Option : StateNode
{
    private static readonly PackedScene TextboxOptionPanel = GD.Load<PackedScene>(
        "res://Scenes/UI/Textbox/Button/TextboxOptionPanel.tscn"
    );

    [Export] public Textbox textbox;
    [Export] public RichTextLabel nameLabel;
    [Export] public RichTextLabel textLabel;
    [Export] public TextureRect portrait;
    [Export] public VBoxContainer buttonsContainer;

    public override void Enter()
    {
        foreach (Option option in textbox.CurrNode.Options)
        {
            var optionPanel = TextboxOptionPanel.Instantiate() as TextboxOptionPanel;
            buttonsContainer.AddChild(optionPanel);

            optionPanel.Label.Text = option.Text;
            optionPanel.Pressed += OnButtonPressed;
        }
    }

    public override void Exit()
    {
        // clear out all the buttons
        foreach (Node btn in buttonsContainer.GetChildren())
            btn.QueueFree();

        // reset the text reveal on the textbox itself
        textbox.TextLabel.VisibleCharacters = 0;
    }

    private void OnButtonPressed(TextboxOptionPanel panel)
    {
        int index = panel.GetIndex();
        Option[] options = textbox.CurrNode.Options;
        textbox.CurrNode = options[index].Next;

        if (textbox.CurrNode != null)
        {
            var signalHub = GetNode<SignalHub>("/root/SignalHub");
            signalHub.EmitSignal(SignalHub.SignalName.TextboxOptionSelected, textbox.CurrNode.Key);
            EmitSignal(SignalName.StateUpdate, "Enabled");
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, "Disabled");
        }
    }
}
