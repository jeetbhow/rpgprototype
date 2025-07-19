namespace Combat.Attack;

public interface IAttacker
{
    bool HasDamage { get; set; }
    int APCost { get; }
    public int ComputeDamage();
}
