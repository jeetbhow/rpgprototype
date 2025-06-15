using Godot;

public partial class SignalHub : Node
{   
    // UI Signals
    [Signal]
    public delegate void DialogueStartedEventHandler(DialogueTree dialogueTree);

    [Signal]
    public delegate void TextboxOptionSelectedEventHandler(string option);

    // Skill Check Signals
    [Signal]
    public delegate void SkillCheckFailedEventHandler();

    [Signal]
    public delegate void SkillCheckPassedEventHandler();

    // Party Signals
    [Signal]
    public delegate void PartyMemberAddedEventHandler(PartyMember partyMember);
}
