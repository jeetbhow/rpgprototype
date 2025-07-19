using Godot;
using System;

using Combat.Actors;
using Combat.Talk;

[GlobalClass]
public partial class Jeff : Enemy
{
    [ExportGroup("Talk Action Responses")]
    [Export]
    public TalkActionResult AskAboutWeakness { get; set; }

    public override TalkActionResult GetTalkActionResult(Player player, TalkAction action)
    {
        return action.ID switch
        {
            TalkActionID.AskAboutWeakness => AskAboutWeakness,
            _ => throw new NotImplementedException($"Talk action {action.ID} not implemented for Jeff.")
        };
    }
}
