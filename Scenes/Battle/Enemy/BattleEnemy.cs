using Godot;
using System.Threading.Tasks;

public partial class BattleEnemy : Node2D
{
    private static readonly Color YellowTint = new(1.0f, 1.0f, 0.0f, 1.0f);
    private static readonly Color NoTint = new(1.0f, 1.0f, 1.0f, 1.0f);

    private AnimatedSprite2D _animatedSprite2D;
    private AnimationPlayer _animationPlayer;

    [Export] public Enemy Data { get; set; }

    public ProgressBar Healthbar { get; set; }
    public GpuParticles2D DeathParticles { get; private set; }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        if (_animationPlayer == null)
        {
            GD.PrintErr("AnimationPlayer not found in Enemy node.");
        }

        DeathParticles = GetNode<GpuParticles2D>("DeathParticles");
        if (DeathParticles == null)
        {
            GD.PrintErr("Particles not found in Enemy node.");
        }

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
        DeathParticles.Restart();
        DeathParticles.Emitting = true;
    }
}
