using Godot;
using System.Collections.Generic;

public partial class Game : Node
{
    private const int _D6Max = 6;
    private const int _D6Min = 1;

    private Player _player;

    public static readonly Color APColor = new("5d76dc");
    public static readonly Color BodySkillColor = new("#D54C4E");

    public static Game Instance { get; private set; }

    [Export]
    public Player Player
    {
        get => _player;
        set
        {
            _player = value;
            Party.Add(_player);
        }
    }

    public List<Ally> Party { get; set; } = [];

    public override void _Ready()
    {
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
