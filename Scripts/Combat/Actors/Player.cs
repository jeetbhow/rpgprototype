using Godot;
using System.Linq;

using Items;

namespace Combat.Actors;

[GlobalClass]
public partial class Player : Ally
{
    [Export]
    public Item[] Inventory { get; set; }

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
}
