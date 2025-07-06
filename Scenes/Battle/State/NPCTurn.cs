using Godot;
using System;
using System.Threading.Tasks;

public partial class NPCTurn : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattlePlayerTurn { get; set; }
    [Export] public StateNode TurnQueueEmpty { get; set; }

    public override async Task Enter()
    {
        Fighter curr = Battle.CurrFighter;
        await Battle.UI.Log.AppendLine($"{curr.Name} is ready to fight!");

        if (curr is Enemy enemy)
        {
            FighterAI ai = enemy.AI;
            while (ai.CanAct(curr))
            {
                AIAction action = ai.PickAction();
                int dmg = -1;

                if (action.HasDmg)
                {
                    int minDmg = action.MinDmg;
                    int maxDmg = action.MaxDmg;

                    // Pick a random number in the range of minDmg and maxDmg
                    dmg = (int)(GD.Randi() % (maxDmg - minDmg + 1)) + minDmg;
                }

                await Battle.UI.Log.AppendLine(action.Message);
                await Task.Delay(500);

                if (dmg != -1)
                {
                    int targetIndex = ai.PickTarget(Battle.Party);
                    if (targetIndex != -1)
                    {
                        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt);
                        Battle.DamageAllyHP(targetIndex, dmg);
                        await Battle.UI.Log.AppendLine($"{curr.Name} dealt {dmg} damage.");
                    }
                }

                curr.AP -= action.APCost;
            }
        }

        if (Battle.TurnQueue.Count > 0)
        {
            Battle.CurrFighter = Battle.TurnQueue.Dequeue();
            switch (Battle.CurrFighter)
            {
                case Player:
                    EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                    break;
                case Ally:
                    EmitSignal(SignalName.StateUpdate, Name);
                    break;
                case Enemy:
                    EmitSignal(SignalName.StateUpdate, Name);
                    break;
                default:
                    throw new InvalidOperationException("Unknown fighter type in turn queue.");
            }
        }
        else
        {
            EmitSignal(SignalName.StateUpdate, TurnQueueEmpty.Name);
        }
    }
}
