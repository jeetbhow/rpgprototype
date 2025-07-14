using Godot;

using Combat;

public partial class TurnQueuePanel : PanelContainer
{
    private TextureRect _portrait;
    private Fighter _fighter;
    private AnimationPlayer _animationPlayer;

    public Fighter Fighter
    {
        get => _fighter;
        set
        {
            _fighter = value;
            if (_portrait != null)
            {
                _portrait.Texture = _fighter.Portrait;
            }
        }
    }

    public override void _Ready()
    {
        _portrait = GetNode<TextureRect>("TextureRect");
        _portrait.Texture = _fighter.Portrait;
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
