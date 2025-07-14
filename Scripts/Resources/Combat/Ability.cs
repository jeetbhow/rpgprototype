using Godot;

namespace Combat;

public enum AbilityName
{
    None,
    KnifeSlash,
    JeffWiggle
}

[GlobalClass]
public partial class Ability : Resource
{
    [Export]
    public int APCost { get; set; }

    [Export]
    public DamageRange DamageRange { get; set; }

    [Export]
    public AbilityName Name { get; set; }

    public int Damage { get; private set; }

    public int RollDamage()
    {
        Damage = GD.RandRange(DamageRange.Min, DamageRange.Max);
        return Damage;
    }
}
