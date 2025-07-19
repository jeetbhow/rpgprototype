using Godot;

using Combat.Talk;
using Combat.Attack;
using Combat.Actors;
using System;
using System.Threading.Tasks;

namespace Signal;

public partial class SignalHub : Node
{
    public static SignalHub Instance { get; private set; }
    public static Task<TEventArgs> WaitForEventAsync<TEventArgs>(
        Action<Action<TEventArgs>> subscribe,
        Action<Action<TEventArgs>> unsubscribe)
    {
        var tcs = new TaskCompletionSource<TEventArgs>();
        void handler(TEventArgs args)
        {
            unsubscribe(handler);
            tcs.SetResult(args);
        }


        subscribe(handler);
        return tcs.Task;
    }
    public static event Action<FighterEventArgs> AttackRequested;
    public static event Action<FighterEventArgs> FighterAttacked;

    public override void _Ready()
    {
        Instance = this;
    }

    // Textbox Signals

    public static void RaiseAttackRequested(Fighter attacker, Fighter defender, IAttacker attack)
    {
        AttackRequested?.Invoke(new FighterEventArgs(
            attacker,
            defender,
            attack
        ));
    }

    public static void RaiseFighterAttacked(Fighter attacker, Fighter defender, IAttacker attack)
    {
        FighterAttacked?.Invoke(new FighterEventArgs(
            attacker,
            defender,
            attack
        ));
    }

    [Signal]
    public delegate void DialogueStartedEventHandler(DialogueTree dialogueTree);

    [Signal]
    public delegate void TextboxOptionSelectedEventHandler(string option);

    [Signal]
    public delegate void FinishedSkillCheckEventHandler(bool result);

    [Signal]
    public delegate void PartyMemberAddedEventHandler(OverworldCharacter partyMember);

    // Combat Signals

    [Signal]
    public delegate void AttackCancelledEventHandler();

    [Signal]
    public delegate void EnemySelectedEventHandler(Enemy enemy, int index);

    [Signal]
    public delegate void FighterStatChangedEventHandler(Fighter fighter, StatType stat, int newValue);

    [Signal]
    public delegate void FighterTurnEndedEventHandler(Fighter fighter);

    [Signal]
    public delegate void CombatLogUpdateRequestedEventHandler(string message);

    [Signal]
    public delegate void CombatLogUpdatedEventHandler();
}
