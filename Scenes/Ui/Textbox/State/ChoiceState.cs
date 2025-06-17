using System;
using System.Threading.Tasks;
using Godot;

public partial class ChoiceState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledStateNode;
    [Export] public StateNode DisabledStateNode;
    [Export] public StateNode SkillCheckStateNode;

    private EventBus _eventBus;
    private SoundManager _soundManager;

    public override void _Ready()
    {
        base._Ready();
        _eventBus = GetNode<EventBus>(EventBus.Path);
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
    }

    public override async Task Enter()
    {
        Textbox.PopulateChoiceContainer();
        foreach (var child in Textbox.ChoiceContainer.GetChildren())
        {
            var choice = child as ChoiceButton;
            choice.Pressed += OnChoiceSelected;
        }
    }

    public override async Task Exit()
    {
        Textbox.CleanUpChoiceContainer();
    }

    private void OnChoiceSelected(ChoiceButton panel)
    {
        if (Textbox.CurrNode is not ChoiceNode node)
        {
            throw new InvalidOperationException("Expected curent node to be an instance of ChoiceNode");
        }
        ChoiceData cd = node.ChoiceData[panel.GetIndex()];
        _ = ProcessChoice(cd);
    }

    private async Task ProcessChoice(ChoiceData cd)
    {
        _soundManager.PlaySfx(SoundManager.Sfx.Confirm);
        await ToSignal(GetTree().CreateTimer(0.2f), Timer.SignalName.Timeout);

        switch (cd.Type)
        {
            case "regular":
                Textbox.CurrNode = cd.Next;
                _eventBus.EmitSignal(EventBus.SignalName.TextboxOptionSelected, Textbox.CurrNode.Key);
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
