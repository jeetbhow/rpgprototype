using Godot;
using Godot.Collections;
using System.Linq;

using Combat.Talk;
using Items;

namespace Combat.Actors;

[GlobalClass]
public partial class Enemy : Fighter
{
    [Export] public SpriteFrames SpriteFrames { get; set; }
    [Export] public FighterAI AI { get; set; }
    [Export] public TalkAction[] TalkActions { get; set; }
    [Export] public Array<Item> HeldItems { get; set; }

    [ExportGroup("Enemy Dialogue")]
    [Export] public string IntroLog { get; set; }
    [Export] public string IntroBalloon { get; set; }
    [Export] public string DeathMsgLog { get; set; }
    [Export] public string DeathMsgBalloon { get; set; }
    [Export] public string[] AttackBalloonText { get; set; }

    public bool WeaknessExposed;

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

        // TODO: Implement smarter targeting logic.
        int index = (int)(GD.Randi() % fighters.Length);
        return fighters[index];
    }
}
