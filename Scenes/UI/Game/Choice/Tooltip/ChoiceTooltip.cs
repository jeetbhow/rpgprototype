using Godot;

public partial class ChoiceTooltip : PanelContainer
{
    [Export] public RichTextLabel SkillLabel { get; set; }
    [Export] public RichTextLabel DifficultyLabel { get; set; }
    [Export] public RichTextLabel OddsLabel { get; set; }
    [Export] public RichTextLabel ProbabilityLabel { get; set; }

    public string SkillName { get; set; }
    public int PlayerSkillValue { get; set; }
    public Color SkillColor { get; set; }
    public int Difficulty { get; set; }
    public string Category { get; set; }
    public float Probability { get; set; }

    public void Update()
    {
        SkillLabel.Text = $"[color={SkillColor.ToHtml()}]{SkillName}: {PlayerSkillValue}[/color]";
        DifficultyLabel.Text = "Difficulty: " + Difficulty;
        OddsLabel.Text = "Odds: " + Category;
        ProbabilityLabel.Text = $"{Probability * 100}%".ToString();
    }

    public void Show()
    {
        Color c = Modulate;
        c.A = 1.0f;
        Modulate = c;
    }

    public void Hide()
    {
        Color c = Modulate;
        c.A = 0.0f;
        Modulate = c;
    }
}
