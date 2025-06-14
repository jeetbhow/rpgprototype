using Godot;
using System;

[GlobalClass]
public partial class DialogueHitbox : Area2D
{
    [Export(PropertyHint.File, "*.json")]
    public string DialogueFile { get; set; }

    [Export]
    public Node2D Parent { get; set; }

    private bool playerNearby = false;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (!playerNearby || !Monitoring)
            return;

        if (@event.IsActionPressed("Accept"))
        {
            DialogueTree tree = TreeBuilder.CreateTree(DialogueFile);
            var signalHub = GetNode<SignalHub>("/root/SignalHub");
            signalHub.EmitSignal(SignalHub.SignalName.DialogueStarted, tree);
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PartyMember)
        {
            playerNearby = true;
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is PartyMember)
        {
            playerNearby = false;
        }
    }
}
