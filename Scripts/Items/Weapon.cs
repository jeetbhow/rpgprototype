using Godot;

using Combat.Attack;

namespace Items;

public enum WeaponType
{
    Bladed,
    Blunt,
}

[GlobalClass]
public partial class Weapon : Item, IAttacker
{
    [Export]
    public int APCost { get; private set; }

    [Export]
    public DamageRange DamageRange { get; private set; }

    [Export]
    public WeaponType WeaponType { get; private set; }

    public int Damage { get; private set; }
    public bool HasDamage { get; set; } = true;

    public int ComputeDamage()
    {
        Damage = DamageRange.Roll();
        return Damage;
    }

}
