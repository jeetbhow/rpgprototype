using Godot;

namespace Combat;

[GlobalClass]
public partial class Enemy : Fighter
{
    [Export] public SpriteFrames SpriteFrames { get; set; }
    [Export] public string IntroLog { get; set; }
    [Export] public string IntroBalloon { get; set; }
    [Export] public string DeathMsgLog { get; set; }
    [Export] public string DeathMsgBalloon { get; set; }
    [Export] public string[] AttackBalloonText { get; set; }
    [Export] public FighterAI AI { get; set; }
}
