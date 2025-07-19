using Signal;

public partial class JeetDialogueHitbox : DialogueHitbox
{
    public override void _Ready()
    {
        base._Ready();
        SignalHub.Instance.TextboxOptionSelected += OnTextboxOptionSelected;
    }

    public void OnTextboxOptionSelected(string option)
    {
        if (option == "Yes" && Parent is OverworldCharacter)
        {
            SignalHub.Instance.EmitSignal(SignalHub.SignalName.PartyMemberAdded, Parent);
            Monitoring = false;
        }
    }

}
