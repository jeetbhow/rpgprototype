using Godot;

[GlobalClass]
public partial class AIAction : Resource
{
    [Export] public string Name { get; set; }
    [Export] public string Message { get; set; }
    [Export] public bool HasDmg { get; set; } = false;
    [Export] public int MinDmg { get; set; } = -1;
    [Export] public int MaxDmg { get; set; } = -1;
    [Export] public int APCost { get; set; } = 0;
}
