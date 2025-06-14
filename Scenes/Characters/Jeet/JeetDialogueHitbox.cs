using Godot;
using System;

public partial class JeetDialogueHitbox : DialogueHitbox
{
    private SignalHub signalHub;

    public override void _Ready()
    {
        base._Ready();
        signalHub = GetNode<SignalHub>("/root/SignalHub");
        signalHub.TextboxOptionSelected += OnTextboxOptionSelected;
    }

    public void OnTextboxOptionSelected(string option)
    {
        if (option == "Yes" && Parent is PartyMember)
        {
            signalHub.EmitSignal(SignalHub.SignalName.PartyMemberAdded, Parent);
            Monitoring = false;
        }
    }

}
