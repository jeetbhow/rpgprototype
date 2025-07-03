using Godot;

[GlobalClass]
public partial class PartyInfoBar : HBoxContainer
{
    private RichTextLabel _label;
    private ProgressBar   _progressBar;
    private StyleBoxFlat _barStyleBox;
    private StyleBoxFlat _fillStyleBox;

    private string _labelText = "??:";
    private double _value     = 0.0;
    private double _maxValue  = 0.0;

    [Export]
    public string Label
    {
        get => _labelText;
        set
        {
            _labelText = value;
            if (_label != null)
                _label.Text = _labelText;
        }
    }

    [Export]
    public double Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateBar();
        }
    }

    [Export]
    public double MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            UpdateBar();
        }
    }

    [Export]
    public StyleBoxFlat BackgroundStyleBox
    {
        get => _progressBar?.GetThemeStylebox("background", "ProgressBar") as StyleBoxFlat;
        set
        {
            _barStyleBox = value;
            _progressBar?.AddThemeStyleboxOverride("background", value);
        }
    }

    [Export]
    public StyleBoxFlat FillStyleBox
    {
        get => _progressBar?.GetThemeStylebox("fill", "ProgressBar") as StyleBoxFlat;
        set
        {
            _fillStyleBox = value;
            _progressBar?.AddThemeStyleboxOverride("fill", value);
        }
    }

    public override void _Ready()
    {
        _label = GetNode<RichTextLabel>("RichTextLabel");
        _progressBar = GetNode<ProgressBar>("ProgressBar");
        _progressBar.AddThemeStyleboxOverride("background", _barStyleBox);
        _progressBar.AddThemeStyleboxOverride("fill", _fillStyleBox);

        // initial draw
        _label.Text = _labelText;
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (_progressBar == null)
            return;

        // set max *before* value, every time
        _progressBar.MaxValue = _maxValue;
        _progressBar.Value    = _value;
    }
}
