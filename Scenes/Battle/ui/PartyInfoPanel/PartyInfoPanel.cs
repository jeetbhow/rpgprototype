using Godot;
using System.Threading.Tasks;

using Signal;
using Combat.Actors;

[GlobalClass]
public partial class PartyInfoPanel : PanelContainer
{
    [Export] public Fighter PartyMember { get; set; }

    private RichTextLabel _nameLabel;
    private StatBar _hpBar, _mpBar, _apBar;

    public override void _Ready()
    {
        SignalHub.AttackRequested += OnAttackRequested;
        SignalHub.Instance.FighterStatChanged += OnFighterStatChanged;

        _nameLabel = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Name");
        _hpBar = GetNode<StatBar>("MarginContainer/VBoxContainer/HPBar");
        _mpBar = GetNode<StatBar>("MarginContainer/VBoxContainer/MPBar");
        _apBar = GetNode<StatBar>("MarginContainer/VBoxContainer/APBar");

        if (PartyMember != null)
        {
            _hpBar.MaxValue = PartyMember.MaxHP;
            _hpBar.Value = PartyMember.HP;
            _mpBar.Value = PartyMember.MP;
            _mpBar.MaxValue = PartyMember.MaxMP;
            _apBar.MaxValue = PartyMember.MaxAP;
            _apBar.Value = PartyMember.AP;
            _nameLabel.Text = PartyMember.Name;
        }
        else
        {
            GD.PushError("Attempted to create a PartyInfoPanel without setting the PartyMember data.");
        }
    }

    public async void OnAttackRequested(FighterEventArgs args)
    {
        if (args.Defender == PartyMember)
        {
            int dmg = args.Attack.ComputeDamage();
            PartyMember.HP -= dmg;

            args.Attacker.AP -= args.Attack.APCost;

            if (args.Defender is not Player)
                SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 8.0f);

            SignalHub.Instance.EmitSignal(SignalHub.SignalName.CombatLogUpdateRequested, $"{args.Defender.Name} took {dmg} damage.");
            await ToSignal(SignalHub.Instance, SignalHub.SignalName.CombatLogUpdated);
            SignalHub.RaiseFighterAttacked(args.Attacker, args.Defender, args.Attack);
        }

        await Task.CompletedTask;
    }

    private void OnFighterStatChanged(Fighter fighter, StatType statType, int newValue)
    {
        if (fighter != PartyMember)
            return;

        StatBar _affectedBar;
        switch (statType)
        {
            case StatType.HP:
                _affectedBar = _hpBar;
                break;
            case StatType.MP:
                _affectedBar = _mpBar;
                break;
            case StatType.AP:
                _affectedBar = _apBar;
                break;
            default:
                GD.PrintErr("PartyInfoPanel: Recieved a signal to change a non-existant stat.");
                return;
        }

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_affectedBar, "Value", newValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

    }
}
