using System;
using System.Threading.Tasks;
using Godot;

public partial class ChoiceState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledStateNode;
    [Export] public StateNode DisabledStateNode;
    [Export] public StateNode SkillCheckStateNode;

    private EventBus eventBus;

    public override void _Ready()
    {
        base._Ready();
        eventBus = GetNode<EventBus>(EventBus.Path);
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
                EmitSignal(SignalName.StateUpdate, EnabledStateNode.Name);
                break;
            case "skill":
                Textbox.CurrSkillCheck = (SkillCheckData)cd;
                EmitSignal(SignalName.StateUpdate, SkillCheckStateNode.Name);
                break;
            case "exit":
                Textbox.CurrNode = null;
                EmitSignal(SignalName.StateUpdate, DisabledStateNode.Name);
                break;
        }
    }
}
