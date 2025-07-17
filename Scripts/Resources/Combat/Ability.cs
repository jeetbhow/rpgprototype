using Godot;

namespace Combat;

public enum AbilityType
{
    None,
    KnifeSlash,
    BaseballBatSwing,
    JeffWiggle,
}

[GlobalClass]
public partial class Ability : Resource
{
    [Export]
    public int APCost { get; private set; }

    [Export]
    public DamageRange DamageRange { get; private set; }

    [Export]
    public AbilityType Type { get; private set; }

    public int Damage { get; private set; }

    public int RollDamage()
    {
        Damage = DamageRange.Roll();
        return Damage;
    }
}
