using System;
using System.Threading.Tasks;
using Godot;

public partial class ChoiceState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledState;
    [Export] public StateNode DisabledState;

    private EventBus eventBus;
    private SkillCheckManager skillCheckManager;

    public override void _Ready()
    {
        base._Ready();
        eventBus = GetNode<EventBus>(EventBus.Path);
        skillCheckManager = GetNode <SkillCheckManager>(SkillCheckManager.Path);
    }

    public override async Task Enter()
    {
        Textbox.PopulateChoiceContainer();
        foreach (var child in Textbox.ChoiceContainer.GetChildren())
        {
            var choice = child as TextboxChoice;
            choice.Pressed += OnChoiceSelected;
        }
    }

    public override async Task Exit()
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
                skillCheckManager.PerformSkillCheck(scd.SkillId, scd.Difficulty);
                break;
            case "exit":
                Textbox.CurrNode = null;
                EmitSignal(SignalName.StateUpdate, DisabledState.Name);
                break;
        }
    }
}
