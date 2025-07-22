using Godot;

namespace Combat.Attack;

public partial class Magic : Resource, IAttack
{
    public DamageRange DamageRange { get; set; }
    public bool LandedCrit { get; set; }
    public float CritChance { get; set; }
    public float CritMultiplier { get; set; }
    public int APCost { get; set; }
    public int MPCost { get; set; }

    public int ComputeDamage(float multiplier = 1.0f)
    {
        int damage = DamageRange.Roll();
        if (GD.Randf() < CritChance)
        {
            damage = Mathf.RoundToInt(damage * CritMultiplier);
        }
        return damage;
    }
}
