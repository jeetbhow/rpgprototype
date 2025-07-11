using Godot;
using System;

using Combat;

[GlobalClass]
public partial class PartyAPBar : HBoxContainer
{

    private ProgressBar _bar;
    private RichTextLabel _label;

    private double _value = 0.0;
    private double _maxValue = 0.0;
    private double _underBarValue = 0.0;

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

    public override void _Ready()
    {
        SignalHub.Instance.FighterStatChanged += OnFighterStatChanged;
        _bar = GetNode<ProgressBar>("ProgressBar");
        _label = GetNode<RichTextLabel>("ProgressBar/RichTextLabel");
        UpdateBar();
    }

    private void OnFighterStatChanged(StatType statType, int newValue)
    {
        if (statType != StatType.AP)
            return;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "Value", newValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }   

    private void UpdateBar()
    {
        if (_bar == null)
            throw new System.InvalidOperationException("ProgressBar is not initialized.");

        // set max *before* value, every time
        _bar.MaxValue = _maxValue;
        _bar.Value = _value;
        _label.Text = $"{Math.Round(_value)}/{Math.Round(_maxValue)}";
    }
}
