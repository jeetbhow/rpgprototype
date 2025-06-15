using Godot;

public partial class ChoiceState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledState;
    [Export] public StateNode DisabledState;

    public override void Enter()
    {
        Textbox.PopulateChoiceContainer();
        foreach (var child in Textbox.ChoiceContainer.GetChildren())
        {
            var choice = child as TextboxChoice;
            choice.Pressed += OnChoiceSelected;
        }
    }

    public override void Exit()
    {
        Textbox.CleanUpChoiceContainer();
    }

    private void OnChoiceSelected(TextboxChoice panel)
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
                EmitSignal(SignalName.StateUpdate, DisabledState.Name);
                break;
        }
    }
}
