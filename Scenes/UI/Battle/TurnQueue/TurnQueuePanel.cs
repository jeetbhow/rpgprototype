using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class TurnQueuePanel : HBoxContainer
{
    private TextureRect _textureRect;
    private Texture2D _texture;

    private Node2D _diceContainer;
    private AnimatedSprite2D _dice1;
    private AnimatedSprite2D _dice2;

    private RichTextLabel _athleticsBonusLabel;
    private AnimationPlayer _animationPlayer;

    private int _athleticsBonus;
    private int _dice1Val = 1;
    private int _dice2Val = 1;

    // A reference to the fighter that this TQP belongs to.
    public Fighter Fighter { get; set; }

    [Export]
    public int AthleticsBonus
    {
        get => _athleticsBonus;
        set
        {
            _athleticsBonus = value;
            if (_textureRect != null)
            {
                _athleticsBonusLabel.Text = $"[color={Game.BodySkillColor.ToHtml()}]+{_athleticsBonus}[/color]";
            }
        }
    }

    [Export]
    public int Dice1
    {
        get => _dice1Val;
        set
        {
            _dice1Val = value;
            _dice1?.Play($"{_dice1Val}");
        }
    }

    [Export]
    public int Dice2
    {
        get => _dice2Val;
        set
        {
            _dice2Val = value;
            _dice2?.Play($"{_dice2Val}");
        }
    }

    [Export]
    public Texture2D Portrait
    {
        get => _texture;
        set
        {
            _texture = value;
            if (_textureRect != null)
            {
                _textureRect.Texture = _texture;
            }
        }
    }

    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("PanelContainer/TextureRect");
        _textureRect.Texture = _texture;

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _athleticsBonusLabel = GetNode<RichTextLabel>("Control/RichTextLabel");
        _athleticsBonusLabel.Text = $"[color={Game.BodySkillColor.ToHtml()}]+{_athleticsBonus}[/color]";

        _diceContainer = GetNode<Node2D>("Control/Dice");
        _dice1 = GetNode<AnimatedSprite2D>("Control/Dice/Dice1");
        _dice2 = GetNode<AnimatedSprite2D>("Control/Dice/Dice2");

        _dice1.Play($"{_dice1Val}");
        _dice2.Play($"{_dice2Val}");
    }

    public void ShowInfo()
    {
        _diceContainer.Visible = true;
        _athleticsBonusLabel.Visible = true;
        _animationPlayer.Play("show_info");
    }

    public void HideInfo()
    {
        _diceContainer.Visible = false;
        _athleticsBonusLabel.Visible = false;
    }
}
