using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

using Combat.Actors;
using System;

public partial class Game : Node
{
    public const int D6Max = 6;
    public const int D6Min = 1;
    public const int WeaponSwapAPCost = 2;

    private Player _player;
    private RandomNumberGenerator _rng = new();

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

    public async Task Wait(int ms)
    {
        await ToSignal(GetTree().CreateTimer(ms / 1000.0), Timer.SignalName.Timeout);
    }

    public float GetSuccessChance(int difficulty)
    {
        return difficulty switch
        {
            < 2 => 1.0f,
            2 => 0.97f,
            3 => 0.94f,
            4 => 0.89f,
            5 => 0.81f,
            6 => 0.70f,
            7 => 0.55f,
            8 => 0.42f,
            9 => 0.31f,
            10 => 0.22f,
            11 => 0.17f,
            12 => 0.03f,
            > 12 => 0.0f
        };
    }

    public ValueTuple<int, int> RollTwoD6()
    {
        int roll1 = _rng.RandiRange(D6Min, D6Max);
        int roll2 = _rng.RandiRange(D6Min, D6Max);
        return (roll1, roll2);
    }

    public bool CombatSpeechCheck(Player player, Enemy enemy, int difficulty)
    {
        ValueTuple<int, int> playerTwoD6 = RollTwoD6();
        int playerD6Sum = playerTwoD6.Item1 + playerTwoD6.Item2;

        ValueTuple<int, int> enemyTwoD6 = RollTwoD6();
        int enemyD6Sum = enemyTwoD6.Item1 + enemyTwoD6.Item2;

        int effectiveDifficulty = (enemyD6Sum + enemy.Rhetoric.Value + difficulty) - (player.Rhetoric.Value + playerD6Sum);
        float successChance = GetSuccessChance(effectiveDifficulty);

        GD.Print($"Combat speech check: {enemyD6Sum + enemy.Rhetoric.Value + difficulty} - {player.Rhetoric.Value + playerD6Sum} = {effectiveDifficulty}, success chance: {successChance}");

        return _rng.Randf() < successChance;
    }

}
