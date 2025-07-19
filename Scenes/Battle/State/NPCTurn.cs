using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Signal;
using Combat.Actors;
using Combat.UI;

namespace Combat.Attack;

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
            Fighter target = enemy.PickTarget([.. Battle.Party]);
            EnemyNode enemyNode = Battle.GetEnemyNode(enemy);
            await enemyNode.Monologue();

            AIAction action;
            while ((action = enemy.PickAction()) != null)
            {
                var attackFinished = SignalHub.WaitForEventAsync<FighterEventArgs>(
                    h => SignalHub.FighterAttacked += h,
                    h => SignalHub.FighterAttacked -= h
                );

                await Battle.UI.Log.AppendLine(action.Message);

                SignalHub.RaiseAttackRequested(curr, target, action);

                await attackFinished;
                await Game.Instance.Wait(800);

                if (action.HasDamage)
                {
                    await Battle.UI.Log.AppendLine($"{target.Name} took {action.ComputeDamage()} damage.");
                }
            }
        }

        EmitSignal(SignalName.StateUpdate, TurnEnd.Name);
    }
}
