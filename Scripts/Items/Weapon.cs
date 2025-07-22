using Godot;

using Combat.Attack;

namespace Items;

public enum PhysicalDamageType
{
    Bladed,
    Blunt,
}

[GlobalClass]
public partial class Weapon : Item, IAttack
{
    [Export] public int APCost { get; private set; }
    [Export] public float CritChance { get; set; }
    [Export] public float CritMultiplier { get; set; }
    [Export] public DamageRange DamageRange { get; set; }
    [Export] public PhysicalDamageType DamageType { get; private set; }
    [Export] public PackedScene Effect { get; set; }
    [Export] public bool HasAnimation { get; set; }
    [Export] public AudioStream Sfx { get; set; }
    [Export] public int SfxVolume { get; set; }
    [Export] public int HitDelayMs { get; set; }

    public int Damage { get; private set; }
    public bool LandedCrit { get; set; } = false;

    public int ComputeDamage(float multiplier = 1.0f)
    {
        LandedCrit = false;
        Damage = Mathf.RoundToInt(DamageRange.Roll() * multiplier);
        if (GD.Randf() < CritChance)
        {
            Damage = Mathf.RoundToInt(Damage * CritMultiplier);
            LandedCrit = true;
        }
        return Damage;
    }

}
