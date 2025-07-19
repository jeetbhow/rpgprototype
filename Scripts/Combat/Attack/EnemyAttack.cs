using Combat.Attack;
using Godot;
using System;

public partial class EnemyAttack : Resource, IAttacker
{
    [Export]
    public int APCost { get; set; }

    [Export]
    public DamageRange DamageRange { get; set; }

    [Export]
    public bool HasDamage { get; set; }

    public int ComputeDamage()
    {
        return DamageRange.Roll();
    }
}   
