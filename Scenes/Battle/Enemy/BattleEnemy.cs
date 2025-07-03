using Godot;
using System;
using System.Threading.Tasks;

public partial class BattleEnemy : Node2D
{
    private static readonly Color YellowTint = new(1.0f, 1.0f, 0.0f, 1.0f);
    private static readonly Color NoTint = new(1.0f, 1.0f, 1.0f, 1.0f);

    [Export] public Enemy Data { get; set; }

    public AnimationPlayer AnimationPlayer { get; private set; }
    public ProgressBar Healthbar { get; set; }

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

        Healthbar = GetNode<ProgressBar>("Healthbar");
        Healthbar.MaxValue = Data.HP;
        Healthbar.Value = Data.HP;
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

    public void Highlight()
    {
        AnimationPlayer.Play("Highlight");
    }

    public void StopAnimation()
    {
        AnimationPlayer.Stop();
    }

    public void TakeDamage(int damage)
    {   
        double toValue = Mathf.Clamp(Healthbar.Value - damage, 0, Data.HP);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(Healthbar, "value", toValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }
}
