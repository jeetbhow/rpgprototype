using Godot;
using System;

namespace Combat.Attack;

[GlobalClass]
public partial class DamageRange : Resource
{
    [Export]
    public int Max { get; set; }
    [Export]
    public int Min { get; set; }
    [Export]
    public float CritChance { get; set; }
    [Export]
    public float CritMultiplier { get; set; }

    /// <summary>
    /// Compute a random damage value between the values in the damage range and
    /// applies crit if a crit occurs. If the computed damage is a decimal, then it's
    /// converted to the nearest integer.
    /// </summary>
    /// <returns>The computed damage from the damage roll.</returns>
    public int Roll()
    {
        float dmg = GD.RandRange(Min, Max);
        if (CritChance >= GD.Randf())
        {
            dmg *= CritMultiplier;
        }
        return (int)Math.Round(dmg);
    }
}
