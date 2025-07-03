using Godot;

[GlobalClass]
public partial class PartyInfoPanel : PanelContainer
{
    private RichTextLabel _nameLabel;
    private PartyHPBar _hpBar;
    private PartyAPBar _apBar;

    // backing fields for exported properties
    private string _name = "";
    private double _hp, _maxHp, _ap, _maxAp, _apUnderBarValue;

    public AnimationPlayer AnimationPlayer { get; private set; }

    [Export]
    public string PartyMemberName
    {
        get => _name;
        set
        {
            _name = value;
            if (_nameLabel != null)
                _nameLabel.Text = _name;
        }
    }

    [Export]
    public double HP
    {
        get => _hp;
        set
        {
            _hp = value;
            if (_hpBar != null)
                _hpBar.Value = _hp;
        }
    }

    [Export]
    public double MaxHP
    {
        get => _maxHp;
        set
        {
            _maxHp = value;
            if (_hpBar != null)
                _hpBar.MaxValue = _maxHp;
        }
    }

    [Export]
    public double AP
    {
        get => _ap;
        set
        {
            _ap = value;
            if (_apBar != null)
                _apBar.Value = _ap;
        }
    }

    [Export]
    public double MaxAP
    {
        get => _maxAp;
        set
        {
            _maxAp = value;
            if (_apBar != null)
                _apBar.MaxValue = _maxAp;
        }
    }

    public override void _Ready()
    {
        _nameLabel = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Name");
        _hpBar = GetNode<PartyHPBar>("MarginContainer/VBoxContainer/PartyHPBar");
        _apBar = GetNode<PartyAPBar>("MarginContainer/VBoxContainer/PartyAPBar");

        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        // now apply whatever was set before Ready() ran:
        _nameLabel.Text = _name;
        _hpBar.Value = _hp;
        _hpBar.MaxValue = _maxHp;
        _apBar.Value = _ap;
        _apBar.MaxValue = _maxAp;
    }
}
