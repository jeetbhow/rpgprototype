using Godot;

namespace Combat.Talk;

[GlobalClass]
public partial class TalkActionResult : Resource
{
    [Export] public string StartLogEntry { get; set; }
    [Export] public string SuccessBalloonText { get; set; }
    [Export] public string FailureBalloonText { get; set; }
    [Export] public string SuccessLogEntry { get; set; }
    [Export] public string FailureLogEntry { get; set; } = "It didn't work.";

    public bool IsSuccess { get; private set; }
}
