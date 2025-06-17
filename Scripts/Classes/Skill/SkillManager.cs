using Godot;
using System;

public static class SkillManager
{
    private static Color DarkRed = Color.Color8(242, 48, 48);
    private static Color LightRed = Color.Color8(240, 79, 79);
    private static Color Orange = Color.Color8(242, 184, 48);
    private static Color Yellow = Color.Color8(245, 236, 76);
    private static Color DarkYellow = Color.Color8(255, 245, 51);
    private static Color LimeGreen = Color.Color8(200, 245, 132);
    private static Color Green = Color.Color8(96, 240, 105);
    private static Color LightBlue = Color.Color8(79, 133, 240);
    private static Color DarkPurple = Color.Color8(97, 27, 133);

    private const int _d6Max = 6;
    private const int _d6Min = 1;

    public static Color GetSkillColor(SkillType type)
    {
        return type switch
        {
            SkillType.Strength => DarkRed,
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid skill id.")
        };
    }

    public static string GetSkillName(SkillType type)
    {
        return type switch
        {
            SkillType.Strength => "Strength",
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid skill id.")
        };
    }

    public static bool PerformSkillCheck(PlayerData playerData, SkillCheckData skillCheckData)
    {
        SkillType skillType = skillCheckData.Skill.Type;
        int playerModifier = skillType switch
        {
            SkillType.Strength => playerData.Strength.Value,
            _ => throw new ArgumentException($"Skill check data has an invalid skill type of {skillType}")
        };

        RandomNumberGenerator rng = new();
        rng.Randomize();
        int r1 = rng.RandiRange(_d6Min, _d6Max);
        int r2 = rng.RandiRange(_d6Min, _d6Max);

        int difficulty = skillCheckData.Skill.Value - playerModifier;
        return r1 + r2 >= difficulty;
    }

    public static float GetSuccessChance(PlayerData playerData, SkillCheckData skillCheckData)
    {
        SkillType type = skillCheckData.Skill.Type;
        int playerModifier = playerData.GetSkill(type).Value;

        int difficulty = skillCheckData.Skill.Value - playerModifier;
        return difficulty switch
        {
            < 2 => 1.0f,
            2 => 0.97f,
            3 => 0.94f,
            4 => 0.89f,
            5 => 0.81f,
            6 => 0.70f,
            7 => 0.55f,
            8 => 0.42f,
            9 => 0.31f,
            10 => 0.22f,
            11 => 0.17f,
            12 => 0.03f,
            > 12 => 0.0f
        };
    }

    public static string GetProbabilityCategory(float p)
    {
        return p switch
        {
            >= 0.0f and <= 0.10f => $"[color={DarkRed.ToHtml()}]Very Low[/color]",
            > 0.10f and <= 0.30f => $"[color={Orange.ToHtml()}]Low[/color]",
            > 0.40f and <= 0.60f => $"[color={Yellow.ToHtml()}]Fair[/color]",
            > 0.60f and <= 0.80f => $"[color={LimeGreen.ToHtml()}]High[/color]",
            > 0.80f and <= 1.00f => $"[color={Green.ToHtml()}]Very High[/color]",
            _ => throw new ArgumentOutOfRangeException(nameof(p))
        };
    }
}
