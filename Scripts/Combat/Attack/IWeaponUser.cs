using Godot;
using System;

using Combat.Actors;
using Combat.AI;
using Items;

namespace Combat.Attack;

public interface IWeaponUser
{
    string Name { get; set; }
    Weapon Weapon { get; set; }

    private const string _basePath = "res://Resources/Items/Weapons/";

    public NPCWeaponAttackAction CreateNPCWeaponAttackAction()
    {
        NPCWeaponAttackAction action = new();
        action.Weapon = Weapon.ID switch
        {
            ItemID.Knife => ResourceLoader.Load<Weapon>(_basePath + "Knife.tres"),
            ItemID.BaseballBat => ResourceLoader.Load<Weapon>(_basePath + "BaseballBat.tres"),
            ItemID.Katana => ResourceLoader.Load<Weapon>(_basePath + "Katana.tres"),
            _ => throw new NotSupportedException($"IWeaponUser: The weapon with ID {Weapon.ID} does not have an NPCWeaponAttackAction."),
        };

        return action;
    }
    public string CreateLogEntry(Fighter defender)
    {
        return Weapon.ID switch
        {
            ItemID.Knife => $"{Name} lunges at {defender.Name} with their knife.",
            ItemID.BaseballBat => $"{Name} winds up and prepares to deliver a blow to {defender.Name}.",
            ItemID.Katana => $"{Name} draws their blade and prepares to strike {defender.Name}.",
            _ => throw new InvalidOperationException($"PartyMember: weapon {Weapon.ID} not supported."),
        };
    }
}
