using Godot;

namespace Combat;

public enum AbilityName
{
    KnifeSlash
}

[GlobalClass]
public partial class Ability : Resource
{
    [Export]
    public int APCost { get; set; }
    [Export]
    public DamageRange DamageRange { get; set; }

    [Export]
    public AbilityName Name { get; set; }
}
