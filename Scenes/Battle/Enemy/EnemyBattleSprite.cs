using Godot;
using System.Threading.Tasks;

public partial class EnemyBattleSprite : Node2D
{
    private AnimatedSprite2D _animatedSprite2D;
    private AnimationPlayer _animationPlayer;
    private Sprite2D _shadow;

    [Export] public Enemy Data { get; set; }

    public ProgressBar Healthbar { get; set; }
    public GpuParticles2D DeathParticles { get; private set; }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _shadow = GetNode<Sprite2D>("Shadow");

        DeathParticles = GetNode<GpuParticles2D>("DeathParticles");
        Healthbar = GetNode<ProgressBar>("Healthbar");
        Healthbar.MaxValue = Data.HP;
        Healthbar.Value = Data.HP;
    }

    public void Highlight()
    {
        _animationPlayer.Play("Highlight");
    }

    public void StopAnimation()
    {
        _animationPlayer.Stop();
    }

    public void TakeDamage(int damage)
    {
        double toValue = Mathf.Clamp(Healthbar.Value - damage, 0, Data.HP);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(Healthbar, "value", toValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
    }

    public void Die()
    {
        _animatedSprite2D.Visible = false;
        _animationPlayer.Play("destroy_extras");
        DeathParticles.Restart();
        DeathParticles.Emitting = true;
    }
}
