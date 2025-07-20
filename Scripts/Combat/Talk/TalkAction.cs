using System;
using Godot;

namespace Combat.Talk;

public enum TalkActionType
{
    Regular,
    WeaknessExposed,
}

[GlobalClass]
public partial class TalkAction : Resource
{
    [Export] public string Text { get; set; }
    [Export] public string Description { get; set; }
    [Export] public int Difficulty { get; set; }
    [Export] public bool Visible { get; set; } = true;
    [Export] public TalkActionType Type { get; set; } = TalkActionType.Regular;
    [Export] public TalkActionResult Result { get; set; }
}
