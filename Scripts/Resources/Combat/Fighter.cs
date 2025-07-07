using Godot;

[GlobalClass]
public partial class Fighter : Resource
{
    public bool IsDead {
        get { return HP <= 0; }
    }

    [Export] public Texture2D Portrait { get; set; }
    [Export] public string Name { get; set; }
    [Export] public int Level { get; set; }
    [Export] public int HP { get; set; }
    [Export] public int MaxHP { get; set; }
    [Export] public int AP { get; set; }
    [Export] public int MaxAP { get; set; }
    [Export] public int Strength { get; set; }
    [Export] public int Endurance { get; set; }
    [Export] public int Athletics { get; set; }
    [Export] public int Initiative { get; set; } = 0;   // Default value, set by the game logic.
}
