using Godot;

public partial class TextboxChoice : PanelContainer
{
    [Signal]
    public delegate void PressedEventHandler(TextboxChoice panel);

    [Export]
    public RichTextLabel RichTextLabel { get; set; }

    private Color defaultColor;

    public override void _Ready()
    {
        var baseStyle = (StyleBoxFlat)GetThemeStylebox("panel").Duplicate();
        defaultColor = baseStyle.BgColor;
        MouseEntered += OnHoverStart;
        MouseExited += OnHoverEnd;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb
            && mb.Pressed
            && mb.ButtonIndex == MouseButton.Left)
        {
            EmitSignal(SignalName.Pressed, this);
        }
    }

    private void OnHoverStart()
    {
        var stylebox = (StyleBoxFlat)GetThemeStylebox("panel").Duplicate();
        EnableBorder(stylebox);
        AddThemeStyleboxOverride("panel", stylebox);
    }

    private void OnHoverEnd()
    {
        var stylebox = (StyleBoxFlat)GetThemeStylebox("panel").Duplicate();
        DisableBorder(stylebox);
        AddThemeStyleboxOverride("panel", stylebox);
    }

    private static void SetupContentMargins(StyleBoxFlat stylebox)
    {
        stylebox.ContentMarginLeft = 0;
        stylebox.ContentMarginRight = 0;
        stylebox.ContentMarginTop = 0;
        stylebox.ContentMarginBottom = 0;
    }

    private static void DisableBorder(StyleBoxFlat stylebox)
    {
        stylebox.BorderWidthLeft = 0;
        stylebox.BorderWidthRight = 0;
        stylebox.BorderWidthTop = 0;
        stylebox.BorderWidthBottom = 0;
    }

    private static void EnableBorder(StyleBoxFlat stylebox)
    {
        stylebox.BorderWidthLeft = 2;
        stylebox.BorderWidthRight = 2;
        stylebox.BorderWidthTop = 2;
        stylebox.BorderWidthBottom = 2;
    }
}
