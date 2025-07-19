using System.Linq;
using Godot;

using Combat.Talk;

namespace Combat.Actors;

[GlobalClass]
public abstract partial class Enemy : Fighter
{
    [Export]
    public SpriteFrames SpriteFrames { get; set; }
    [Export]
    public FighterAI AI { get; set; }
    [Export]
    public TalkAction[] TalkActions { get; set; }
    [Export]
    public string IntroLog { get; set; }
    [Export]
    public string IntroBalloon { get; set; }
    [Export]
    public string DeathMsgLog { get; set; }
    [Export]
    public string DeathMsgBalloon { get; set; }
    [Export]
    public string[] AttackBalloonText { get; set; }

    public AIAction PickAction()
    {
        var candidates = AI.Actions
            .Where(a => a.CanExecute(AP))
            .ToArray();

        if (candidates.Length == 0)
            return null;

        int idx = GD.RandRange(0, candidates.Length - 1);
        return candidates[idx];
    }

    public Ally PickTarget(Ally[] fighters)
    {
        if (fighters.Length == 0) return null;

        // Simple random selection for now
        int index = (int)(GD.Randi() % fighters.Length);
        return fighters[index];
    }

    public abstract TalkActionResult GetTalkActionResult(Player player, TalkAction action);
}
