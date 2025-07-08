using Combat;
using Godot;

public partial class SignalHub : Node
{
    public static SignalHub Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    [Signal]
    public delegate void DialogueStartedEventHandler(DialogueTree dialogueTree);

    [Signal]
    public delegate void TextboxOptionSelectedEventHandler(string option);

    [Signal]
    public delegate void FinishedSkillCheckEventHandler(bool result);

    [Signal]
    public delegate void PartyMemberAddedEventHandler(OverworldCharacter partyMember);

    [Signal]
    public delegate void AttackRequestedEventHandler(Fighter attacker, Fighter defender, Ability ability);
}
