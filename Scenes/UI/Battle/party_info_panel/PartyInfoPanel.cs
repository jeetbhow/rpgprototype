using Godot;

using Combat;

[GlobalClass]
public partial class PartyInfoPanel : PanelContainer
{
    public Ally PartyMember { get; set; }

    private RichTextLabel _nameLabel;
    private PartyHPBar _hpBar;
    private PartyAPBar _apBar;

    public override void _Ready()
    {
        SignalHub.Instance.FighterAttacked += OnFighterAttacked;
        _nameLabel = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Name");
        _hpBar = GetNode<PartyHPBar>("MarginContainer/VBoxContainer/PartyHPBar");
        _apBar = GetNode<PartyAPBar>("MarginContainer/VBoxContainer/PartyAPBar");

        _hpBar.MaxValue = PartyMember.MaxHP;
        _hpBar.Value = PartyMember.HP;
        _apBar.MaxValue = PartyMember.MaxAP;
        _apBar.Value = PartyMember.AP;
        _nameLabel.Text = PartyMember.Name;
    }

    public void OnFighterAttacked(Fighter attacker, Fighter defender, Ability ability)
    {
        if (attacker == PartyMember)
        {
            PartyMember.AP -= ability.APCost;
        }
    }
}
