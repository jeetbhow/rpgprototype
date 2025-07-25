using System;

using Combat.Actors;
using Combat.Attack;

namespace Signal;

/// <summary>
/// Carries data for when a fighter has been attacked:
///   • Who attacked,
///   • Who was attacked,
///   • What attack was used.
/// </summary>
public class FighterEventArgs : EventArgs
{
    public Fighter Attacker { get; }
    public Fighter Defender { get; }
    public IAttack Attack { get; }

    public FighterEventArgs(
        Fighter attacker,
        Fighter defender,
        IAttack attack)
    {
        Attacker = attacker;
        Defender = defender;
        Attack = attack;
    }
}