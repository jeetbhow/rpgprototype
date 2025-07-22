using Godot;
using Godot.Collections;

using Items;
using Combat.Actors;
using Combat.AI;

public partial class NPCFighter : Fighter
{
    [ExportGroup("AI")]
    [Export] public Array<NPCAction> NPCActions { get; set; }
    [Export] public NPCBehaviour NPCBehaviour { get; set; }

    [ExportGroup("Items")]
    [Export] public Array<Item> HeldItems { get; set; }

    public bool HasEnoughAP()
    {
        return AP > 0;
    }
}
