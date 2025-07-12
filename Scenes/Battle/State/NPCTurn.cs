using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Combat;

public partial class NPCTurn : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode TurnEnd { get; set; }

    public override async Task Enter()
    {
        Fighter curr = Battle.CurrFighter;
        await Battle.UI.Log.AppendLine($"{curr.Name} is ready to fight!");

        if (curr is Enemy enemy)
        {
            FighterAI ai = enemy.AI;

            List<EnemyBattleSprite> sprites = [.. Battle.EnemyNodes.GetChildren().Cast<EnemyBattleSprite>()];
            EnemyBattleSprite sprite = sprites.Find(sprite => sprite.Enemy == enemy);
            await sprite.Monologue();

            while (ai.CanAct(curr))
            {
                AIAction action = ai.PickAction();
                int dmg = -1;

                if (action.HasDmg)
                {
                    int minDmg = action.Ability.DamageRange.Min;
                    int maxDmg = action.Ability.DamageRange.Max;

                    // Pick a random number in the range of minDmg and maxDmg
                    dmg = GD.RandRange(minDmg, maxDmg);
                }

                await Battle.UI.Log.AppendLine(action.Message);
                await Task.Delay(500);

                if (dmg != -1)
                {
                    int targetIndex = ai.PickTarget(Battle.Party);
                    Fighter target = Battle.Party[targetIndex];
                    if (targetIndex != -1)
                    {
                        SignalHub.Instance.EmitSignal(
                            SignalHub.SignalName.FighterAttacked,
                            curr,
                            target,
                            action.Ability);

                        await Battle.UI.Log.AppendLine($"{curr.Name} dealt {dmg} damage.");
                    }
                }
            }
        }

        EmitSignal(SignalName.StateUpdate, TurnEnd.Name);
    }
}
