using System;
using System.Collections.Generic;
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

    
    public AIAction PickAction()
    {
        if (AI.Actions.Length == 0) return null;

        int index = (int)(GD.Randi() % AI.Actions.Length);

        int count = 0;
        while (count < AI.Actions.Length)
        {
            var action = AI.Actions[index];
            if (action.Enabled)
            {
                if (action.CanExecute(AP))
                    return action;
                else
                    action.Enabled = false;
            }

            index = (index + 1) % AI.Actions.Length;
            count++;
        }

        return null; // No valid action found
    }

    public static Ally PickTarget(Ally[] fighters)
    {
        if (fighters.Length == 0) return null;

        // Simple random selection for now
        int index = (int)(GD.Randi() % fighters.Length);
        return fighters[index];
    }

}
