using Godot;
using Combat;
using System;

namespace Items;

public enum WeaponID
{
    None,
    Knife,
    BaseballBat,
}

[GlobalClass]
public partial class Weapon : Item
{
    private static readonly string[] WeaponStrings =
    [
        "Knife",
        "Baseball Bat",
    ];

    public static string GetWeaponName(WeaponID id)
    {
        if (id == WeaponID.None || id >= WeaponID.Knife && id <= WeaponID.BaseballBat)
        {
            return WeaponStrings[(int)id];
        }
        throw new ArgumentException($"Weapon: Invalid WeaponID: {id}");
    }

    public static WeaponID GetWeaponID(string weaponName)
    {
        for (int i = 0; i < WeaponStrings.Length; i++)
        {
            if (WeaponStrings[i].Equals(weaponName, StringComparison.OrdinalIgnoreCase))
            {
                return (WeaponID)i;
            }
        }
        return WeaponID.None;
    }

    [Export]
    public Ability Ability { get; set; }

    [Export]
    public WeaponID ID { get; set; }
}
