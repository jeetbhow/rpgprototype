using Godot;

using Items;
using Combat.Attack;

namespace Combat.Actors;

[GlobalClass]
public partial class PartyMember : NPCFighter, IWeaponUser
{
    [Export] public Weapon Weapon { get; set; }
}
