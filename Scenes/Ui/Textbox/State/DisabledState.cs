using Godot;

public partial class DisabledState : StateNode
{
    [Export]
    public Textbox Textbox;

    public override async void Enter()
    {
        GetTree().Paused = false;
        Textbox.Visible = false;

        var signalHub = GetNode<SignalHub>("/root/SignalHub");
        var result  = await ToSignal(signalHub, SignalHub.SignalName.DialogueStarted);

        var dialogueTree = (DialogueTree)result[0];
        Textbox.Tree     = dialogueTree;
        Textbox.CurrNode = dialogueTree.Root;

        EmitSignal(nameof(StateUpdate), "EnabledState");
    }

    public override void Exit()
    {
        GetTree().Paused = true;
        Textbox.Visible = true;
        Textbox.CurrNode = Textbox.Tree.Root;
    }
}
