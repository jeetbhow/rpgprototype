public partial class Fighter
{
    // Base class for all participants in a battle, such as enemies and players.
    // This class can be extended to add specific functionality for different types of participants.
    public enum FighterType
    {
        Player,
        Ally,
        Enemy
    }

    public string Name { get; set; }
    public FighterType Type { get; set; }
    public int HP { get; set; }
    public int AP { get; set; }
    public int Strength { get; set; }
    public int Endurance { get; set; }
    public int Athletics { get; set; }
    public int Initiative {get; set;}

    public Fighter(
        string name,
        FighterType type,
        int hp,
        int ap,
        int strength,
        int endurance,
        int athletics)
    {
        Name = name;
        Type = type;
        HP = hp;
        AP = ap;
        Strength = strength;
        Endurance = endurance;
        Athletics = athletics;
        Initiative = 0; // Default value, can be set later based on game logic
    }
}
