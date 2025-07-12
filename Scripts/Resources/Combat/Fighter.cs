using Godot;

namespace Combat;

public enum StatType
{
    HP,
    AP,
}


[GlobalClass]
public partial class Fighter : Resource
{
    public bool IsDead
    {
        get { return HP <= 0; }
    }

    [Export]
    public Texture2D Portrait { get; set; }

    [Export]
    public string Name { get; set; }

    [Export]
    public int Level { get; set; }

    [Export]
    public int HP
    { 
        get { return _hp; }
        set
        {
            _hp = Mathf.Max(0, value);
            SignalHub.Instance?.EmitSignal(SignalHub.SignalName.FighterStatChanged, this, (int)StatType.HP, _hp);
        }
    }

    [Export]
    public int MaxHP { get; set; }

    [Export]
    public int AP
    {
        get { return _ap; }
        set
        {
            _ap = Mathf.Max(0, value);
            SignalHub.Instance?.EmitSignal(SignalHub.SignalName.FighterStatChanged, this, (int)StatType.AP, _ap);
        }
    }

    [Export]
    public int MaxAP { get; set; }

    [Export]
    public int Strength { get; set; }

    [Export]
    public int Endurance { get; set; }

    [Export]
    public int Athletics { get; set; }

    [Export]
    public int Initiative { get; set; } = 0;   // Default value, set by the game logic.

    private int _ap, _hp;
}
