using Godot;

namespace Combat.Talk;

public enum TalkActionID
{
    AskAboutWeakness
}

[GlobalClass]
public partial class TalkAction : Resource
{
    [Export]
    public TalkActionID ID { get; set; }

    [Export]
    public string Text { get; set; }

    [Export]
    public string Description { get; set; }

    [Export]
    public int Difficulty { get; set; }
}
