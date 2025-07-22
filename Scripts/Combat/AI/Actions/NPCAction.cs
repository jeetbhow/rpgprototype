using Godot;

namespace Combat.AI;

[GlobalClass]
public partial class NPCAction : Resource
{
    [Export] public int APCost { get; set; }
    [Export] public int MPCost { get; set; }
    [Export] public string LogEntry { get; set; }
}
