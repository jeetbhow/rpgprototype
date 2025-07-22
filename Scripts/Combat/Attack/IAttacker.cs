namespace Combat.Attack;

public interface IAttack
{
    DamageRange DamageRange { get; set; }
    bool LandedCrit { get; set; }
    float CritChance { get; set; }
    float CritMultiplier { get; set; }
    int APCost { get; set; }
    int MPCost { get; set; }

    public int ComputeDamage(float multiplier = 1.0f);
}
