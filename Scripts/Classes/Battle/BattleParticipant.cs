public partial class BattleParticipant
{
    // Base class for all participants in a battle, such as enemies and players.
    // This class can be extended to add specific functionality for different types of participants.
    public enum BattleParticipantType
    {
        PartyMember,
        Enemy
    }

    public string Name { get; set; }
    public BattleParticipantType Type { get; set; } = BattleParticipantType.PartyMember;
    public int HP { get; set; }
    public int AP { get; set; }
    public int Strength { get; set; }
    public int Endurance { get; set; }
    public int Athletics { get; set; }

    public BattleParticipant(
        string name,
        BattleParticipantType type,
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
    }
}
