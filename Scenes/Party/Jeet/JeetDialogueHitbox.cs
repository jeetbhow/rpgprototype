using Godot;
using System;

public partial class JeetDialogueHitbox : DialogueHitbox
{
    private EventBus signalHub;

    public override void _Ready()
    {
        base._Ready();
        signalHub = GetNode<EventBus>(EventBus.Path);
        signalHub.TextboxOptionSelected += OnTextboxOptionSelected;
    }

    public void OnTextboxOptionSelected(string option)
    {
        if (option == "Yes" && Parent is PartyMember)
        {
            signalHub.EmitSignal(EventBus.SignalName.PartyMemberAdded, Parent);
            Monitoring = false;
        }
    }

}
