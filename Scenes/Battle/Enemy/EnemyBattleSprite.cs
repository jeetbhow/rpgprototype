using Godot;
using System;
using System.Threading.Tasks;

using Combat;

public partial class EnemyBattleSprite : Node2D
{
    [Export]
    public Enemy Enemy { get; set; }

    [Export]
    public float BlinkSeconds = 1.0f;

    private readonly Random _rng = new();

    private AnimatedSprite2D _animatedSprite2D;
    private AnimatedSprite2D _effects;
    private AnimationPlayer _animationPlayer;
    private Sprite2D _shadow;
    private RichTextLabel _hpLabel;
    private Timer _hpTimer;
    private GpuParticles2D _bloodParticles;
    private ShaderMaterial _shader;
    
    private int _prev;           // Index of the previous attack dialogue entry.

    public ChatBallloon ChatBallloon { get; private set; }
    public ProgressBar Healthbar { get; set; }
    public GpuParticles2D DeathParticles { get; private set; }

    public override void _Ready()
    {
        SignalHub.Instance.EnemySelected += OnEnemySelected;
        SignalHub.Instance.AttackCancelled += OnAttackCancelled;
        SignalHub.Instance.AttackRequested += OnAttackRequested;

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite");
        _animatedSprite2D.SpriteFrames = Enemy.SpriteFrames;

        _shader = (ShaderMaterial)_animatedSprite2D.Material;

        _effects = GetNode<AnimatedSprite2D>("Effects");

        _bloodParticles = GetNode<GpuParticles2D>("Blood");

        _shadow = GetNode<Sprite2D>("Shadow");
        _hpLabel = GetNode<RichTextLabel>("Healthbar/RichTextLabel");

        _hpTimer = GetNode<Timer>("HPTimer");
        _hpTimer.Timeout += OnHPTimerTimeout;

        ChatBallloon = GetNode<ChatBallloon>("ChatBalloon");
        DeathParticles = GetNode<GpuParticles2D>("DeathParticles");
        Healthbar = GetNode<ProgressBar>("Healthbar");
        Healthbar.MaxValue = Enemy.HP;
        Healthbar.Value = Enemy.HP;
        _hpLabel.Text = $"{Enemy.HP}/{Enemy.MaxHP}";
    }

    public void OnEnemySelected(Enemy enemy, int index)
    {
        if (enemy == Enemy)
        {
            ShowHP();
        }
        else
        {
            HideHP();
        }
    }

    public void OnAttackCancelled()
    {
        HideHP();
    }

    public async void OnAttackRequested(Fighter attacker, Fighter defender, Ability ability)
    {
        if (attacker == Enemy)
        {
            Enemy.AP -= ability.APCost;
        }
        else if (defender == Enemy)
        {
            await PlayEffects(ability);
            SignalHub.Instance.EmitSignal(
                SignalHub.SignalName.FighterAttacked,
                attacker,
                defender,
                ability
            );

            // TODO - Change this later to use the DamageRange from the ability.
            await TakeDamage(2);
        }
    }

    public async Task PlayEffects(Ability ability)
    {
        if (ability.Name == AbilityName.KnifeSlash)
        {
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.Slash);
            _effects.Play("slash");
            await ToSignal(_effects, "animation_finished");
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt);
            Bleed();
            Blink();
        }
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
        double toValue = Mathf.Clamp(Healthbar.Value - damage, 0, Enemy.HP);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(Healthbar, "value", toValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        _hpLabel.Text = $"{Math.Round(toValue)}/{Enemy.MaxHP}";

        await ToSignal(tween, "finished");
    }

    public async Task Monologue()
    {
        int index = _rng.Next(Enemy.AttackBalloonText.Length);
        if (index == _prev)
        {
            // This should prevent repeated dialogue from popping up during fights.
            index = (index + 1) % Enemy.AttackBalloonText.Length;
        }

        await ChatBallloon.PlayMessage("[shake rate=50 level=5]" + Enemy.AttackBalloonText[index] + "[/shake]", 700);
        _prev = index;
    }

    public async Task Introduction()
    {
        await ChatBallloon.PlayMessage("[shake rate=50 level=5]" + Enemy.IntroBalloon + "[/shake]", 700);
    }

    public async Task Die()
    {
        _animatedSprite2D.Visible = false;
        _animationPlayer.Play("destroy_extras");

        DeathParticles.Restart();
        DeathParticles.Emitting = true;

        // force at least one idle frame so you can actually SEE the particles start
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ChatBallloon.PlayMessage("[shake rate=50 level=5]" + Enemy.DeathMsgBalloon + "[/shake]", 700, false);

        var timer = GetTree().CreateTimer(0.2);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

    public void Blink()
    {
        _shader.SetShaderParameter("blinking", true);
        _shader.SetShaderParameter("elapsed_time", Time.GetTicksMsec() / 1000.0f);
        GetTree().CreateTimer(BlinkSeconds).Timeout += () =>
        {
            _shader.SetShaderParameter("blinking", false);
        };
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
