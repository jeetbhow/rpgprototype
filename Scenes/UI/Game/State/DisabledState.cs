using Godot;
using System.Threading.Tasks;

using Signal;

public partial class DisabledState : StateNode
{
    [Export] public UI UI;
    [Export] public StateNode EnabledStateNode;

    public override void _Ready()
    {
        base._Ready();
    }

    public override async Task Enter()
    {
        UI.Stop();
        var result = await ToSignal(SignalHub.Instance, SignalHub.SignalName.DialogueStarted);

        DialogueTree dialogueTree = (DialogueTree)result[0];
        UI.LoadDialogue(dialogueTree);

        UI.Start();
        EmitSignal(nameof(StateUpdate), EnabledStateNode.Name);
    }

    public override async Task Exit()
    {
        await Task.CompletedTask;
    }
}
