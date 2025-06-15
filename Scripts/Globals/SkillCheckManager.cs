using Godot;

public partial class SkillCheckManager : Node
{


    private EventBus eventBus;

    public override void _Ready()
    {
        eventBus = GetNode<EventBus>(EventBus.Path);
    }

}
