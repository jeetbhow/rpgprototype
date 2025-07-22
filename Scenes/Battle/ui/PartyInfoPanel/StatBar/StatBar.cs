using Godot;
using System;

using Combat.Actors;

[GlobalClass]
public partial class StatBar : HBoxContainer
{
    [Export] public StatType Stat;
    [Export] public StyleBoxFlat Background;
    [Export] public StyleBoxFlat Fill;

    public double Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateBar();
        }
    }
    public double MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            UpdateBar();
        }
    }

    private ProgressBar _bar;
    private RichTextLabel _statLabel;
    private RichTextLabel _valueLabel;

    private double _value = 0.0;
    private double _maxValue = 0.0;
    private double _underBarValue = 0.0;

    public override void _Ready()
    {
        _bar = GetNode<ProgressBar>("ProgressBar");
        _statLabel = GetNode<RichTextLabel>("RichTextLabel");
        _valueLabel = GetNode<RichTextLabel>("ProgressBar/RichTextLabel");

        switch (Stat)
        {
            case StatType.HP:
                _statLabel.Text = "HP";
                break;
            case StatType.MP:
                _statLabel.Text = "MP";
                break;
            case StatType.AP:
                _statLabel.Text = "AP";
                break;
        }

        _bar.AddThemeStyleboxOverride("background", Background);
        _bar.AddThemeStyleboxOverride("fill", Fill);

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (_bar == null)
            throw new System.InvalidOperationException("ProgressBar is not initialized.");

        // set max *before* value, every time
        _bar.MaxValue = _maxValue;
        _bar.Value = _value;
        _valueLabel.Text = $"{Math.Round(_value)}/{_maxValue}";
    }
}
