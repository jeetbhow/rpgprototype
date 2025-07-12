using Godot;
using System.Collections.Generic;

namespace Combat;

[GlobalClass]
public partial class FighterAI : Resource
{
    [Export] public AIAction[] Actions { get; set; }

    public AIAction PickAction()
    {
        if (Actions.Length == 0) return null;

        // Simple random selection for now
        return Actions[GD.Randi() % Actions.Length];
    }

    public int PickTarget(List<Ally> fighters)
    {
        if (fighters.Count == 0) return -1;

        // Simple random selection for now
        return (int)(GD.Randi() % fighters.Count);
    }

    public bool CanAct(Fighter fighter)
    {
        foreach (AIAction action in Actions)
        {
            if (action.Ability.APCost <= fighter.AP)
            {
                return true;
            }
        }
        return false;
    }
}
