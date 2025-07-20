using Godot;

using Combat.Actors;

namespace Combat.Talk;

[GlobalClass]
public partial class TalkAction : Resource
{
    [Export] public string Text { get; set; }
    [Export] public string Description { get; set; }
    [Export] public int Difficulty { get; set; }
    [Export] public bool Visible { get; set; } = true;
    [Export] public bool Enabled { get; set; } = true;
    [Export] public TalkActionResult Result { get; set; }

    public virtual bool IsVisible(Player player, Enemy enemy)
    {
        return Visible;
    }

    public virtual bool IsEnabled(Player player, Enemy enemy)
    {
        return Enabled;
    }
}
