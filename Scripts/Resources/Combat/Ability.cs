using Godot;

namespace Combat;

public enum AbilityName
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
    public AbilityName Name { get; private set; }

    public int Damage { get; private set; }

    public int RollDamage()
    {
        Damage = DamageRange.Roll();
        return Damage;
    }
}
