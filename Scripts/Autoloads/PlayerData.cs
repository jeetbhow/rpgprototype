using System;
using Godot;

[GlobalClass]
public partial class PlayerData : Resource
{
    [Export] public Skill Strength { get; set; }

    public int GetSkillValue(SkillType type)
    {
        return type switch
        {
            SkillType.Strength => Strength.Value,
            _ => throw new ArgumentException($"Attempted to pass in an invalid skill type: ${type}")
        };
    }
}
