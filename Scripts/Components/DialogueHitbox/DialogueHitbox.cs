using Godot;

[GlobalClass]
public partial class DialogueHitbox : Area2D
{
    [Export(PropertyHint.File, "*.json")] public string DialogueFile { get; set; }
    [Export] public Node2D Parent { get; set; }

    private bool playerNearby = false;
    private EventBus eventBus;

    public override void _Ready()
    {
        base._Ready();
        eventBus = GetNode<EventBus>(EventBus.Path);
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
            eventBus.EmitSignal(EventBus.SignalName.DialogueStarted, tree);
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
