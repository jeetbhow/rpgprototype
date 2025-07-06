using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class DiceRollInfo : HBoxContainer
{
    private TextureRect _portrait;
    private Texture2D _portraitTexture;

    private Node2D _diceContainer;
    private AnimatedSprite2D _dice1Anim;
    private AnimatedSprite2D _dice2Anim;

    private RichTextLabel _skillBonusLabel;
    private AnimationPlayer _animationPlayer;

    private Timer _lifetimeTimer;

    private int _skillBonus;
    private int _dice1 = 1;
    private int _dice2 = 1;

    [Export]
    public SkillType SkillType { get; set; }

    [Export]
    public int Bonus
    {
        get => _skillBonus;
        set
        {
            _skillBonus = value;
            if (_portrait != null)
            {
                _skillBonusLabel.Text = $"[color={Game.BodySkillColor.ToHtml()}]+{_skillBonus}[/color]";
            }
        }
    }

    [Export]
    public Texture2D Portrait
    {
        get => _portraitTexture;
        set
        {
            _portraitTexture = value;
            if (_portrait != null)
                _portrait.Texture = _portraitTexture;
        }
    }

    [Export]
    public int Dice1
    {
        get => _dice1;
        set
        {
            _dice1 = value;
            _dice1Anim?.Play($"{_dice1}");
        }
    }

    [Export]
    public int Dice2
    {
        get => _dice2;
        set
        {
            _dice2 = value;
            _dice2Anim?.Play($"{_dice2}");
        }
    }

    public override void _Ready()
    {
        _portrait = GetNode<TextureRect>("Portrait");
        _portrait.Texture = _portraitTexture;

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _skillBonusLabel = GetNode<RichTextLabel>("Control/RichTextLabel");
        _skillBonusLabel.Text = $"[color={Game.BodySkillColor.ToHtml()}]+{_skillBonus}[/color]";

        _diceContainer = GetNode<Node2D>("Control/Dice");
        _dice1Anim = GetNode<AnimatedSprite2D>("Control/Dice/Dice1");
        _dice2Anim = GetNode<AnimatedSprite2D>("Control/Dice/Dice2");

        _lifetimeTimer = GetNode<Timer>("LifetimeTimer");
        _lifetimeTimer.Timeout += OnLifetimeEnded;

        _dice1Anim.Play($"{_dice1}");
        _dice2Anim.Play($"{_dice2}");
    }

    public new void Show()
    {
        _animationPlayer.Play("fade_in");
        Visible = true;
        _lifetimeTimer.Start();
    }

    public async void OnLifetimeEnded()
    {
        await Hide();
    }

    public new async Task Hide()
    {
        _animationPlayer.Play("fade_out");
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        QueueFree();
    }
}
