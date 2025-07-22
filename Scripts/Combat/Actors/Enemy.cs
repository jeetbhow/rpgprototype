using Godot;

using Items;
using Combat.Talk;

namespace Combat.Actors;

[GlobalClass]
public partial class Enemy : NPCFighter
{
    [Export] public SpriteFrames SpriteFrames { get; set; }

    [ExportCategory("Talk")]
    [Export] public TalkAction[] TalkActions { get; set; }

    [ExportGroup("Resistances and Weaknesses")]
    [Export] public PhysicalDamageType PhysicalWeakness { get; set; }
    [Export] public PhysicalDamageType PhysicalResistance { get; set; }

    [ExportGroup("Enemy Dialogue")]
    [Export] public string IntroLog { get; set; }
    [Export] public string IntroBalloon { get; set; }
    [Export] public string DeathMsgLog { get; set; }
    [Export] public string DeathMsgBalloon { get; set; }
    [Export] public string[] AttackBalloonText { get; set; }

    public bool WeaknessExposed;
}
