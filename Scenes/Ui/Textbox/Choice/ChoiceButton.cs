using Godot;

public partial class ChoiceButton : RichTextLabel
{
    [Signal] public delegate void PressedEventHandler(ChoiceButton panel);

    [Export] public StyleBoxFlat Style { get; set; }
    [Export] public StyleBoxFlat HoverStyle { get; set; }
    [Export] public RichTextLabel Label { get; set; }

    public override void _Ready()
    {
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

    public void SetColor(Color color)
    {
        Label.AddThemeColorOverride("default_color", color);
    }

    private void OnHoverStart()
    {
        AddThemeStyleboxOverride("panel", HoverStyle);
    }

    private void OnHoverEnd()
    {
        AddThemeStyleboxOverride("panel", Style);
    }
}
