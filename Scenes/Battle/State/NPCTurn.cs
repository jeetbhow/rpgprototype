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
                var attackFinished = ToSignal(
                    SignalHub.Instance,
                    SignalHub.SignalName.FighterAttacked);

                await Battle.UI.Log.AppendLine(action.Message);

                SignalHub.Instance.EmitSignal(
                    SignalHub.SignalName.AttackRequested,
                    curr,
                    target,
                    action.Ability);

                await attackFinished;
                await Game.Instance.Wait(500);

                if (action.HasDmg)
                {
                    await Battle.UI.Log.AppendLine($"{target.Name} took {action.Ability.Damage} damage.");
                }
            }
        }
        
        EmitSignal(SignalName.StateUpdate, TurnEnd.Name);
    }
}
