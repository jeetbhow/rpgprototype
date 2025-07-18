using Godot;

using Combat;
using System.Threading.Tasks;

[GlobalClass]
public partial class PartyInfoPanel : PanelContainer
{
    public Ally PartyMember { get; set; }

    private RichTextLabel _nameLabel;
    private PartyHPBar _hpBar;
    private PartyAPBar _apBar;

    public override void _Ready()
    {
        SignalHub.Instance.AttackRequested += OnAttackRequested;
        SignalHub.Instance.FighterStatChanged += OnFighterStatChanged;
        _nameLabel = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Name");
        _hpBar = GetNode<PartyHPBar>("MarginContainer/VBoxContainer/PartyHPBar");
        _apBar = GetNode<PartyAPBar>("MarginContainer/VBoxContainer/PartyAPBar");

        _hpBar.MaxValue = PartyMember.MaxHP;
        _hpBar.Value = PartyMember.HP;
        _apBar.MaxValue = PartyMember.MaxAP;
        _apBar.Value = PartyMember.AP;
        _nameLabel.Text = PartyMember.Name;
    }

    public async void OnAttackRequested(Fighter attacker, Fighter defender, Ability ability)
    {
        if (defender == PartyMember)
        {
            if (ability.DamageRange != null)
            {
                int dmg = ability.RollDamage();
                PartyMember.HP -= dmg;
            }

            attacker.AP -= ability.APCost;
            SignalHub.Instance.EmitSignal(
                SignalHub.SignalName.FighterAttacked,
                attacker,
                defender,
                ability
            );
        }

        await Task.CompletedTask;
    }

    private void OnFighterStatChanged(Fighter fighter, StatType statType, int newValue)
    {
        if (fighter != PartyMember)
            return;

        if (statType == StatType.AP)
        {
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_apBar, "Value", newValue, 1.0f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);
        }
        else if (statType == StatType.HP)
        {
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_hpBar, "Value", newValue, 1.0f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);
        }
    }
}
