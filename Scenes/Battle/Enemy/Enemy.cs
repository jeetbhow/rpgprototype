using Godot;
using System.Threading.Tasks;

public partial class Enemy : Node2D
{
    [Export] public EnemyData Data { get; set; }

    public AnimationPlayer AnimationPlayer { get; private set; }

    public override void _Ready()
    {
        // Enemies are initially invisible.
        Color color = Modulate;
        color.A = 0.0f;
        Modulate = color;

        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        if (AnimationPlayer == null)
        {
            GD.PrintErr("AnimationPlayer not found in Enemy node.");
            return;
        }
    }

    public async Task FadeIn()
    {
        SoundManager soundManager = SoundManager.Instance;
        soundManager.PlaySfx(SoundManager.Sfx.Poof);
        await ToSignal(soundManager.SfxPlayer, "finished");

        AnimationPlayer.Play("FadeIn");
        await ToSignal(AnimationPlayer, "animation_finished");
    }

    public async Task FadeOut()
    {
        AnimationPlayer.Play("FadeOut");
        await ToSignal(AnimationPlayer, "animation_finished");
    }
}
