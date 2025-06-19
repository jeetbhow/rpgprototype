using Godot;
using System.Threading.Tasks;

public partial class DisabledState : StateNode
{
    [Export] public UI UI;
    [Export] public StateNode EnabledStateNode;

    private EventBus eventBus;

    public override void _Ready()
    {
        base._Ready();
        eventBus = GetNode<EventBus>(EventBus.Path);
    }

    public override async Task Enter()
    {
        UI.Stop();
        var result = await ToSignal(eventBus, EventBus.SignalName.DialogueStarted);

        DialogueTree dialogueTree = (DialogueTree)result[0];
        UI.LoadDialogue(dialogueTree);

        UI.Start();
        EmitSignal(nameof(StateUpdate), EnabledStateNode.Name);
    }

    public override async Task Exit()
    {}
}
