using Godot;

[GlobalClass]
public partial class Fighter : Resource
{
    // Base class for all participants in a battle, such as enemies and players.
    // This class can be extended to add specific functionality for different types of participants.
    public enum FighterType
    {
        Player,
        Ally,
        Enemy
    }

    public bool IsDead {
        get { return HP <= 0; }
    }

    [Export] public string Name { get; set; }
    [Export] public int Level { get; set; }
    [Export] public int HP { get; set; }
    [Export] public int MaxHP { get; set; }
    [Export] public int AP { get; set; }
    [Export] public int MaxAP { get; set; }
    [Export] public int Strength { get; set; }
    [Export] public int Endurance { get; set; }
    [Export] public int Athletics { get; set; }
    [Export] public int Initiative { get; set; }
}
