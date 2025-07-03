using Godot;

public partial class Ally : Fighter
{
    [Export] public int MaxHP { get; set; }
    [Export] public int MaxAP { get; set; }
    [Export] public int Level { get; set; }
}
