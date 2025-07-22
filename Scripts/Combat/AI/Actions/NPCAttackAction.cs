using Godot;

using Combat.Attack;
using Combat.AI;

[GlobalClass]
public partial class NPCAttackAction : NPCAction, IAttack
{
    [Export] public DamageRange DamageRange { get; set; }
    [Export] public float CritChance { get; set; }
    [Export] public float CritMultiplier { get; set; }

    public bool LandedCrit { get; set; }

    public int ComputeDamage(float multiplier = 1.0f)
    {
        LandedCrit = false;
        int damage = Mathf.RoundToInt(DamageRange.Roll() * multiplier);
        if (GD.Randf() < CritChance)
        {
            damage = Mathf.RoundToInt(damage * CritMultiplier);
            LandedCrit = true;
        }
        return damage;
    }
}
