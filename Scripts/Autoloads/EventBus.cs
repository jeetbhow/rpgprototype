using Godot;

public partial class EventBus : Node
{
    public const string Path = "/root/EventBus";
    // UI Signals
    [Signal]
    public delegate void DialogueStartedEventHandler(DialogueTree dialogueTree);

    [Signal]
    public delegate void TextboxOptionSelectedEventHandler(string option);

    // Skill Check Signals
    [Signal]
    public delegate void FinishedSkillCheckEventHandler(bool result);

    // Party Signals
    [Signal]
    public delegate void PartyMemberAddedEventHandler(PartyMember partyMember);
}
