using Godot;
using System.Collections.Generic;

public partial class Game : Node
{
    public static Game Instance { get; private set; }

    public List<PartyMemberData> Party { get; set; } = [];

    public override void _Ready()
    {
        // Initialize the party with some default members.
        PartyMemberData EriData = new()
        {
            Name = "Eri",
            Level = 1,
            HP = 10,
            MaxHP = 10,
            AP = 5,
            MaxAP = 5,
            Strength = 5,
            Endurance = 5,
            Athletics = 5
        };

        AddPartyMember(EriData);
       
        Instance = this;
    }

    public void AddPartyMember(PartyMemberData member)
    {
        Party.Add(member);
    }

    public void RemovePartyMember(PartyMemberData member)
    {
        Party.Remove(member);
    }
}
