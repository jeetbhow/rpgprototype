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
                        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 8.0f);
                        Battle.ShakeCamera(1, 5);
                        Battle.DamageAllyHP(targetIndex, dmg);
                        await Battle.UI.Log.AppendLine($"{curr.Name} dealt {dmg} damage.");
                    }
                }

                curr.AP -= action.APCost;
            }
        }

        EmitSignal(SignalName.StateUpdate, TurnEnd.Name);
    }
}
