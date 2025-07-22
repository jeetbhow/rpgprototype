using Godot;

using Items;

namespace Combat.AI;

[GlobalClass]
public partial class NPCWeaponAttackAction : NPCAction
{
    [Export] public Weapon Weapon { get; set; }
}
