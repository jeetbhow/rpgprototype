using Godot;
using System.Collections.Generic;

public partial class Game : Node
{
    private const int _D6Max = 6;
    private const int _D6Min = 1;
    public static readonly Color APColor = new("5d76dc");

    public static Game Instance { get; private set; }

    public List<Ally> Party { get; set; } = [];

    public override void _Ready()
    {
        // Initialize the party with some default members.
        Player EriData = new()
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

    public void AddPartyMember(Ally member)
    {
        Party.Add(member);
    }

    public void RemovePartyMember(Ally member)
    {
        Party.Remove(member);
    }
}
