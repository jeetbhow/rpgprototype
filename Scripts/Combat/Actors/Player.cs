using Godot;
using Godot.Collections;
using System.Linq;

using Items;
using Combat.Attack;
using Combat.AI;

namespace Combat.Actors;

[GlobalClass]
public partial class Player : Fighter, IWeaponUser
{
    [Export] public Weapon Weapon { get; set; }
    [Export] public Array<Item> Inventory { get; set; }
    [Export] public int Money { get; set; }

    /// <summary>
    /// Returns all weapons in the player's inventory.
    /// </summary>
    /// <returns>An array of weapons in the player's inventory.</returns>
    public Weapon[] GetWeapons()
    {
        return Inventory.Where(item => item is Weapon weapon && weapon.ID != ItemID.None)
                        .Select(item => (Weapon)item)
                        .ToArray();
    }

    public NPCWeaponAttackAction CreateNPCWeaponAttackAction()
    {
        // Players are controllable, so they don't use NPC actions.
        return null;
    }
}