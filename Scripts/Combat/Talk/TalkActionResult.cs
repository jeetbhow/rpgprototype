using Godot;

namespace Combat.Talk;

public enum TalkActionEffect
{
    None,
    RevealWeakness,
    Surrender,
    Death,
}

[GlobalClass]
public partial class TalkActionResult : Resource
{
    [Export] public TalkActionEffect Effect { get; set; } = TalkActionEffect.None;

    [ExportGroup("Log Text")]
    [Export] public string InitialLogEntry { get; set; }
    [Export] public string SuccessLogEntry { get; set; }
    [Export] public string FailureLogEntry { get; set; } = "It didn't work.";

    [ExportGroup("Balloon Text")]
    [Export] public string SuccessBalloonText { get; set; }
    [Export] public string FailureBalloonText { get; set; }


    public bool IsSuccess { get; private set; }
}
