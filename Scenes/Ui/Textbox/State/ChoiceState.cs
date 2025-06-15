using Godot;

public partial class ChoiceState : StateNode
{
    private static readonly PackedScene TextboxOptionPanel = GD.Load<PackedScene>(
        "res://Scenes/UI/Textbox/Button/TextboxOptionPanel.tscn"
    );

    [Export] public Textbox Textbox;
    [Export] public RichTextLabel NameLabel;
    [Export] public RichTextLabel TextLabel;
    [Export] public TextureRect Portrait;
    [Export] public VBoxContainer ButtonsContainer;
    [Export] public StateNode EnabledState;
    [Export] public StateNode DisabledState;

    public override void Enter()
    {
        ChoiceNode node = (ChoiceNode)Textbox.CurrNode;
        foreach (ChoiceData choice in node.ChoiceData)
        {
            var optionPanel = TextboxOptionPanel.Instantiate() as TextboxOptionPanel;
            ButtonsContainer.AddChild(optionPanel);
            optionPanel.Label.Text = choice.Text;
            optionPanel.Pressed += OnButtonPressed;
        }
    }

    public override void Exit()
    {
        foreach (Node btn in ButtonsContainer.GetChildren())
        {
            btn.QueueFree();
        }
        Textbox.TextLabel.VisibleCharacters = 0;
    }

    private void OnButtonPressed(TextboxOptionPanel panel)
    {
        int index = panel.GetIndex();
        ChoiceNode node = (ChoiceNode)Textbox.CurrNode;
        ChoiceData cd = node.ChoiceData[index];
        var signalHub = GetNode<SignalHub>("/root/SignalHub");

        switch (cd.Type)
        {
            case "regular":
                Textbox.CurrNode = cd.Next;

                signalHub.EmitSignal(SignalHub.SignalName.TextboxOptionSelected, Textbox.CurrNode.Key);
                EmitSignal(SignalName.StateUpdate, EnabledState.Name);
                break;
            case "skill":
                SkillCheckData scd = (SkillCheckData)cd;

                RandomNumberGenerator rng = new();
                rng.Randomize();
                int roll = rng.RandiRange(1, 20);

                GD.Print(scd.Difficulty);

                if (roll >= scd.Difficulty)
                {
                    signalHub.EmitSignal(SignalHub.SignalName.SkillCheckPassed);
                    Textbox.CurrNode = scd.SuccessNext;
                    EmitSignal(SignalName.StateUpdate, EnabledState.Name);
                }
                else
                {
                    Textbox.CurrNode = scd.FailNext;
                    signalHub.EmitSignal(SignalHub.SignalName.SkillCheckFailed);
                    EmitSignal(SignalName.StateUpdate, EnabledState.Name);
                }
                GD.Print($"You rolled a {roll}!");
                break;
            case "exit":
                Textbox.CurrNode = null;
                EmitSignal(SignalName.StateUpdate, EnabledState.Name);
                break;
        }
    }
}
