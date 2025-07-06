using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyBattleSprite : Node2D
{
    private AnimatedSprite2D _animatedSprite2D;
    private AnimationPlayer _animationPlayer;
    private Sprite2D _shadow;
    private RichTextLabel _hpLabel;
    private Timer _hpTimer;

    [Export] public Enemy Data { get; set; }

    public ProgressBar Healthbar { get; set; }
    public GpuParticles2D DeathParticles { get; private set; }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _shadow = GetNode<Sprite2D>("Shadow");
        _hpLabel = GetNode<RichTextLabel>("Healthbar/RichTextLabel");

        _hpTimer = GetNode<Timer>("HPTimer");
        _hpTimer.Timeout += OnHPTimerTimeout;

        DeathParticles = GetNode<GpuParticles2D>("DeathParticles");
        Healthbar = GetNode<ProgressBar>("Healthbar");
        Healthbar.MaxValue = Data.HP;
        Healthbar.Value = Data.HP;
        _hpLabel.Text = $"{Data.HP}/{Data.MaxHP}";
    }

    public void ShowHP()
    {
        Healthbar.Visible = true;
        _animationPlayer.Play("hp_appear");
    }

    public void HideHP()
    {
        _animationPlayer.Play("hp_hide");
        _hpTimer.Start();
    }

    public void OnHPTimerTimeout()
    {
        Healthbar.Visible = false;
    }

    public void TakeDamage(int damage)
    {
        double toValue = Mathf.Clamp(Healthbar.Value - damage, 0, Data.HP);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(Healthbar, "value", toValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        _hpLabel.Text = $"{Math.Round(toValue)}/{Data.MaxHP}";
    }

    public async Task Die()
    {
        // hide the sprite-frame, play any “destroy_extras” animation…
        _animatedSprite2D.Visible = false;
        _animationPlayer.Play("destroy_extras");

        // restart & emit the GPU particles
        DeathParticles.Restart();
        DeathParticles.Emitting = true;

        // force at least one idle frame so you can actually SEE the particles start
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // grab the configured lifetime of the particle system
        // GPUParticles2D has a `Lifetime` property in seconds
        double deathTime = DeathParticles.Lifetime;

        // now wait that exact amount without blocking Godot’s main loop
        var timer = GetTree().CreateTimer(deathTime);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }
}
