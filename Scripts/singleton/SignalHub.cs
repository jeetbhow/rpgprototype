using Godot;

using Combat;

public partial class SignalHub : Node
{
    public static SignalHub Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    // Textbox Signals

    [Signal]
    public delegate void DialogueStartedEventHandler(DialogueTree dialogueTree);

    [Signal]
    public delegate void TextboxOptionSelectedEventHandler(string option);

    [Signal]
    public delegate void FinishedSkillCheckEventHandler(bool result);

    [Signal]
    public delegate void PartyMemberAddedEventHandler(OverworldCharacter partyMember);

    // Combat Signals

    [Signal]
    public delegate void AttackRequestedEventHandler(Fighter attacker, Fighter defender, Ability ability);

    [Signal]
    public delegate void FighterAttackedEventHandler(Fighter attacker, Fighter defender, Ability ability);

    [Signal]
    public delegate void AttackCancelledEventHandler();

    [Signal]
    public delegate void EnemySelectedEventHandler(Enemy enemy, int index);

    [Signal]
    public delegate void FighterStatChangedEventHandler(Fighter fighter, StatType stat, int newValue);
}
