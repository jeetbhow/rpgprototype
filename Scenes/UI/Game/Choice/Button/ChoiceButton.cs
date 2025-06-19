using Godot;

public partial class ChoiceButton : HBoxContainer
{
    [Export] public TextureRect Arrow { get; set; }
    [Export] public RichTextLabel Label { get; set; }
    
    public Choice Choice { get; set; }

    public override void _Ready()
    {
        Label.ClipContents = false;
        Label.FitContent = true;
    }

    public void HideArrow()
    {
        Color color = Arrow.SelfModulate;
        color.A = 0.0f;
        Arrow.SelfModulate = color;
    }

    public void ShowArrow()
    {
        Color color = Arrow.SelfModulate;
        color.A = 1.0f;
        Arrow.SelfModulate = color;
    }

    public void SetColor(Color color)
    {
        Label.Text = $"[color={color.ToHtml()}]{Label.Text}[/color]";
    }
}
