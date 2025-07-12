using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Combat;

public partial class NPCTurn : StateNode
{
    [Export]
    public Battle Battle { get; set; }

    [Export]
    public StateNode TurnEnd { get; set; }

    public override async Task Enter()
    {
        Fighter curr = Battle.CurrFighter;
        await Battle.UI.Log.AppendLine($"{curr.Name} is ready to fight!");

        if (curr is Enemy enemy)
        {
            Fighter target = Enemy.PickTarget([.. Battle.Party]);

            List<EnemyBattleSprite> sprites = [.. Battle.EnemyNodes.GetChildren().Cast<EnemyBattleSprite>()];
            EnemyBattleSprite sprite = sprites.Find(sprite => sprite.Enemy == enemy);
            await sprite.Monologue();

            AIAction action;
            while ((action = enemy.PickAction()) != null)
            {
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

                SignalHub.Instance.EmitSignal(
                    SignalHub.SignalName.AttackRequested,
                    curr,
                    target,
                    action.Ability);

                if (dmg != -1)
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

        GD.Print($"NPCTurn: {curr.Name} finished their turn.");
    }
}
