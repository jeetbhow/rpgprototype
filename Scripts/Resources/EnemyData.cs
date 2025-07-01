using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string Name { get; set; }
    [Export] public string Introduction { get; set; }
    [Export] public int HP { get; set; }
    [Export] public int SP { get; set; }
    [Export] public int AP { get; set; }
    [Export] public int Athletics { get; set; }
    [Export] public int Strength { get; set; }
    [Export] public int Endurance { get; set; }
}
