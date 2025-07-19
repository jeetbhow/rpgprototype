using Godot;
using System;

public enum SkillType
{
    None,
    Strength,
    Endurance,
    Athletics,
    Rhetoric,
    Logic,
    Knowledge,
    Authority,
    Empathy,
    Charisma,
    Dexterity,
    Perception,
    Reflexes
}

[GlobalClass]
public partial class Skill : Resource
{
    [Export]
    public SkillType Type { get; set; }

    [Export]
    public int Value { get; set; }

    public Skill()
    {
        Type = SkillType.None;
        Value = 0;
    }

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
