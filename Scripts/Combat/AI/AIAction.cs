using Godot;

using Combat.Attack;

namespace Combat;

[GlobalClass]
public partial class AIAction : Resource, IAttacker
{
    [Export]
    public string Message { get; set; }

    [Export]
    public DamageRange DamageRange { get; set; }

    [Export]
    public int APCost { get; set; }

    [Export]
    public bool HasDamage { get; set; }

    public bool CanExecute(int apCost)
    {
        return apCost >= APCost;
    }

    public int ComputeDamage()
    {
        return DamageRange.Roll();
    }

}
