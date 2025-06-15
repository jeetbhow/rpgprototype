using Godot;

public partial class DisabledState : StateNode
{
    [Export] public Textbox Textbox;
    [Export] public StateNode EnabledStateNode;

    private SignalHub signalHub;

    public override void _Ready()
    {
        base._Ready();
        signalHub = GetNode<SignalHub>("/root/SignalHub");
    }

    public override async void Enter()
    {
        GetTree().Paused = false;
        Textbox.Visible = false;

        var result = await ToSignal(signalHub, SignalHub.SignalName.DialogueStarted);
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
