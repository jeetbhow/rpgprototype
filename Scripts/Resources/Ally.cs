using Godot;

public partial class Ally : Resource
{
    [Export] public string Name { get; set; }
    [Export] public int Level { get; set; }
    [Export] public int HP { get; set; }
    [Export] public int MaxHP { get; set; }
    [Export] public int AP { get; set; }
    [Export] public int MaxAP { get; set; }
    [Export] public int Strength { get; set; }
    [Export] public int Endurance { get; set; }
    [Export] public int Athletics { get; set; }
}
