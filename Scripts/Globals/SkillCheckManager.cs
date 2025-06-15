using Godot;

public partial class SkillCheckManager : Node
{
    public const string Path = "/root/SkillCheckManager";
    private EventBus eventBus;

    public override void _Ready()
    {
        eventBus = GetNode<EventBus>(EventBus.Path);
    }

    public void PerformSkillCheck(int skillId, int difficulty)
    {
        RandomNumberGenerator rng = new();

        rng.Randomize();
        int roll = rng.RandiRange(1, 20);
        
        if (roll >= difficulty)
        {
            eventBus.EmitSignal(EventBus.SignalName.FinishedSkillCheck, true);
        }
        else
        {
            eventBus.EmitSignal(EventBus.SignalName.FinishedSkillCheck, false);
        }
        GD.Print($"You rolled a {roll}!");
    }

}
