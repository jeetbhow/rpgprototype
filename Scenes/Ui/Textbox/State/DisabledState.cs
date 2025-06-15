using Godot;

public partial class DisabledState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledStateNode;

    private EventBus eventBus;

    public override void _Ready()
    {
        base._Ready();
        eventBus = GetNode<EventBus>("/root/EventBus");
    }

    public override async void Enter()
    {
        GetTree().Paused = false;
        Textbox.Visible = false;

        var result = await ToSignal(eventBus, EventBus.SignalName.DialogueStarted);
        DialogueTree dialogueTree = (DialogueTree)result[0];

        Textbox.Tree = dialogueTree;
        Textbox.CurrNode = dialogueTree.Root;
        EmitSignal(nameof(StateUpdate), EnabledStateNode.Name);
    }

    public override void Exit()
    {
        GetTree().Paused = true;
        Textbox.Visible = true;
        Textbox.CurrNode = Textbox.Tree.Root;
    }
}
