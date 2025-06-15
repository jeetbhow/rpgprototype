using System;
using Godot;

public partial class ChoiceState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledState;
    [Export] public StateNode DisabledState;

    private EventBus eventBus;

    public override void _Ready()
    {
        base._Ready();
        eventBus = GetNode<EventBus>(EventBus.Path);
    }

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
        if (Textbox.CurrNode is not ChoiceNode node)
        {
            throw new InvalidOperationException("Expected curent node to be an instance of ChoiceNode");
        }

        ChoiceData cd = node.ChoiceData[panel.GetIndex()];
        switch (cd.Type)
        {
            case "regular":
                Textbox.CurrNode = cd.Next;
                eventBus.EmitSignal(EventBus.SignalName.TextboxOptionSelected, Textbox.CurrNode.Key);
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
                    eventBus.EmitSignal(EventBus.SignalName.SkillCheckPassed);
                    Textbox.CurrNode = scd.SuccessNext;
                    EmitSignal(SignalName.StateUpdate, EnabledState.Name);
                }
                else
                {
                    Textbox.CurrNode = scd.FailNext;
                    eventBus.EmitSignal(EventBus.SignalName.SkillCheckFailed);
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
