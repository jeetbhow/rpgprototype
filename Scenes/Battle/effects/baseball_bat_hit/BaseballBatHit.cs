using Godot;
using System.Threading.Tasks;

public partial class BaseballBatHit : Sprite2D
{
    public AnimationPlayer AnimationPlayer { get; private set; }

    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
