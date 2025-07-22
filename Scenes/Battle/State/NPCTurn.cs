using Godot;
using System.Threading.Tasks;

using Signal;
using Combat.Actors;
using Combat.UI;
using Combat.AI;
using System;

namespace Combat.Attack;

public partial class NPCTurn : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode TurnEnd { get; set; }

    public override async Task Enter()
    {
        NPCFighter curr = Battle.CurrFighter as NPCFighter;

        await Battle.UI.Log.AppendLine($"{curr.Name} is ready to fight!");

        if (curr is Enemy enemy)
        {
            EnemyNode enemyNode = Battle.GetEnemyNode(enemy);
            await enemyNode.Monologue();
        }

        while (curr.HasEnoughAP())
        {
            (Fighter target, NPCAction action) = curr.NPCBehaviour.DecideTurn(curr, [.. Battle.Enemies], [.. Battle.Party]);

            if (curr is IWeaponUser weaponUser)
                await Battle.UI.Log.AppendLine(weaponUser.CreateLogEntry(target));
            else
                await Battle.UI.Log.AppendLine(action.LogEntry);

            await Game.Instance.Wait(500);

            switch (action)
            {
                case NPCAttackAction attack:
                    await HandleAttack(curr, target, attack);
                    break;
                case NPCWeaponAttackAction weaponAttack:
                    await HandleAttack(curr, target, weaponAttack.Weapon);
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled NPCAction type: {action.GetType().Name}");
            }
        }
        EmitSignal(SignalName.StateUpdate, TurnEnd.Name);
    }

    private static async Task HandleAttack(NPCFighter attacker, Fighter defender, IAttack attack)
    {
        var attackFinished = SignalHub.WaitForEventAsync<FighterEventArgs>(
            h => SignalHub.FighterAttacked += h,
            h => SignalHub.FighterAttacked -= h
        );

        SignalHub.RaiseAttackRequested(attacker, defender, attack);
        await attackFinished;
    }
}
