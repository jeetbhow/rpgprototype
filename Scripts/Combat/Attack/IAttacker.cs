namespace Combat.Attack;

public interface IAttack
{
    DamageRange DamageRange { get; set; }
    bool LandedCrit { get; set; }
    float CritChance { get; set; }
    float CritMultiplier { get; }
    int APCost { get; }

    public int ComputeDamage(float multiplier = 1.0f);
}
