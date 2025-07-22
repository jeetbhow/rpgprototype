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
        switch (Weapon.ID)
        {
            case ItemID.Knife:
                action.Weapon = ResourceLoader.Load<Weapon>(_basePath + "Knife.tres");
                break;
        }
        return action;
    }
    public string CreateLogEntry(Fighter defender)
    {
        return Weapon.ID switch
        {
            ItemID.Knife => $"{Name} lunges at {defender.Name} with their knife.",
            ItemID.BaseballBat => $"{Name} swings their baseball bat at {defender.Name}.",
            _ => throw new InvalidOperationException($"PartyMember: weapon {Weapon.ID} not supported."),
        };
    }
}
