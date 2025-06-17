using Godot;

public partial class TextboxChoice : PanelContainer
{
    [Signal] public delegate void PressedEventHandler(TextboxChoice panel);

    [Export] public PackedScene ToolTipScene { get; set; }
    [Export] public RichTextLabel RichTextLabel { get; set; }

    public SkillCheckData SkillCheckData { get; set; }

    private StyleBoxFlat _defaultStyle = GD.Load<StyleBoxFlat>("res://Resources/Styles/UIPanel.tres");
    private StyleBoxFlat _hoverStyle = GD.Load<StyleBoxFlat>("res://Resources/Styles/UIPanelHover.tres");

    public override void _Ready()
    {
        Theme.SetStylebox("TooltipPanel", "panel", _defaultStyle);
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
        AddThemeStyleboxOverride("panel", _hoverStyle);
    }

    private void OnHoverEnd()
    {
        AddThemeStyleboxOverride("panel", _defaultStyle);
    }
}
