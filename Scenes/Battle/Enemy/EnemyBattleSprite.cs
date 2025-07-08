using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyBattleSprite : Node2D
{
    private readonly Random _rng = new();

    private AnimatedSprite2D _animatedSprite2D;
    private AnimatedSprite2D _effects;
    private AnimationPlayer _animationPlayer;
    private Sprite2D _shadow;
    private RichTextLabel _hpLabel;
    private Timer _hpTimer;
    private GpuParticles2D _bloodParticles;

    private int _prev;           // Index of the previous attack dialogue entry.

    [Export] public Enemy Data { get; set; }

    public ChatBallloon ChatBallloon { get; private set; }
    public ProgressBar Healthbar { get; set; }
    public GpuParticles2D DeathParticles { get; private set; }

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite2D.SpriteFrames = Data.SpriteFrames;

        _effects = GetNode<AnimatedSprite2D>("Effects");

        _bloodParticles = GetNode<GpuParticles2D>("Blood");

        _shadow = GetNode<Sprite2D>("Shadow");
        _hpLabel = GetNode<RichTextLabel>("Healthbar/RichTextLabel");

        _hpTimer = GetNode<Timer>("HPTimer");
        _hpTimer.Timeout += OnHPTimerTimeout;

        ChatBallloon = GetNode<ChatBallloon>("ChatBalloon");
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

    public async Task TakeDamage(int damage)
    {
        double toValue = Mathf.Clamp(Healthbar.Value - damage, 0, Data.HP);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(Healthbar, "value", toValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        _hpLabel.Text = $"{Math.Round(toValue)}/{Data.MaxHP}";

        await ToSignal(tween, "finished");
    }

    public async Task Monologue()
    {
        int index = _rng.Next(Data.AttackBalloonText.Length);
        if (index == _prev)
        {
            // This should prevent repeated dialogue from popping up during fights.
            index = (index + 1) % Data.AttackBalloonText.Length;
        }

        await ChatBallloon.PlayMessage("[shake rate=50 level=5]" + Data.AttackBalloonText[index] + "[/shake]", 700);
        _prev = index;
    }

    public async Task Introduction()
    {
        await ChatBallloon.PlayMessage("[shake rate=50 level=5]" + Data.IntroBalloon + "[/shake]", 700);
    }

    public async Task Die()
    {
        _animatedSprite2D.Visible = false;
        _animationPlayer.Play("destroy_extras");

        DeathParticles.Restart();
        DeathParticles.Emitting = true;

        // force at least one idle frame so you can actually SEE the particles start
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ChatBallloon.PlayMessage("[shake rate=50 level=5]" + Data.DeathMsgBalloon + "[/shake]", 700, false);

        var timer = GetTree().CreateTimer(0.2);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

    public async Task PlayEffect(string name)
    {
        _effects.Play(name);
        await ToSignal(_effects, "animation_finished");
    }

    public void Flinch()
    {
        _animationPlayer.Play("flinch");
    }

    public void StopAnimation()
    {
        _animationPlayer.Stop();
    }

    public void Bleed()
    {
        _bloodParticles.Emitting = true;
    }
}
