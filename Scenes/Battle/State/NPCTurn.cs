using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            EnemyBattleSprite sprite = sprites.Find(sprite => sprite.Data == enemy);

            Random rng = new();
            int index = rng.Next(enemy.AttackBalloonText.Length);
            await sprite.ChatBallloon.PlayMessage(enemy.AttackBalloonText[index]);
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
                        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 10);
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
