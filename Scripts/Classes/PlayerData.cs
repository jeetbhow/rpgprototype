using System;
using Godot;

[GlobalClass]
public partial class PlayerData : Resource
{
    [Export] public Skill Strength { get; set; }

    public Skill GetSkill(SkillType type)
    {
        return type switch
        {
            SkillType.Strength => Strength,
            _ => throw new ArgumentException($"Attempted to pass in an invalid skill type: ${type}")
        };
    }
}
