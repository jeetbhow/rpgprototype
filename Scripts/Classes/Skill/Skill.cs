using System;
using Godot;

public enum SkillType
{
    Strength = 0,
}

[GlobalClass]
public partial class Skill : Resource
{
    [Export] public SkillType Type { get; set; } = SkillType.Strength;
    [Export] public int Value { get; set; } = 0;

    public Skill() { } // Default constructor for Godot

    public Skill(SkillType type, int value)
    {
        Type = type;
        Value = value;
    }

    public static string StringFromType(SkillType type)
    {
        return type switch
        {
            SkillType.Strength => "Strength",
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid skill id.")
        };
    }

    public static SkillType TypeFromString(string name)
    {
        return name switch
        {
            "strength" => SkillType.Strength,
            _ => throw new ArgumentOutOfRangeException($"The string '{name}' does not correspond to any valid skill.")
        };
    }
}
