using Godot;
using System;
using System.Threading.Tasks;

using Items;
using Signal;
using Combat.Talk;
using Combat.Actors;

namespace Combat.UI;

public partial class EnemyNode : Node2D
{
    [Export] public Enemy EnemyData { get; set; }
    [Export] public PackedScene KnifeSlashEffect { get; set; }
    [Export] public PackedScene BaseballBatHitEffect { get; set; }
    [Export] public int TextShakeRate = 50;
    [Export] public int TextShakeLevel = 10;
    [Export] public float BlinkSeconds = 1.0f;
    [Export] public float InitialShakeSpeed { get; set; } = 20.0f;
    [Export] public float ShakeIntensity { get; set; } = 1.0f;
    [Export] public float ShakeDuration { get; set; } = 0.5f;

    public ChatBallloon ChatBalloon { get; private set; }
    public ProgressBar Healthbar { get; set; }
    public GpuParticles2D DeathParticles { get; private set; }

    private readonly Random _rng = new();

    private AnimatedSprite2D _animatedSprite2D;
    private AnimationPlayer _animationPlayer;
    private Sprite2D _shadow;
    private RichTextLabel _hpLabel;
    private Timer _hpTimer;
    private GpuParticles2D _bloodParticles;
    private ShaderMaterial _shader;
    private int _prevMonologueIndex;

    public override void _Ready()
    {
        SignalHub.Instance.EnemySelected += OnEnemySelected;
        SignalHub.Instance.AttackCancelled += OnAttackCancelled;
        SignalHub.AttackRequested += OnAttackRequested;
        SignalHub.FighterAttacked += args => HideHP();

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite");
        _animatedSprite2D.SpriteFrames = EnemyData.SpriteFrames;

        _shader = (ShaderMaterial)_animatedSprite2D.Material;

        _bloodParticles = GetNode<GpuParticles2D>("Blood");

        _shadow = GetNode<Sprite2D>("Sprite/Shadow");
        _hpLabel = GetNode<RichTextLabel>("Healthbar/RichTextLabel");

        _hpTimer = GetNode<Timer>("HPTimer");
        _hpTimer.Timeout += OnHPTimerTimeout;

        ChatBalloon = GetNode<ChatBallloon>("ChatBalloon");
        DeathParticles = GetNode<GpuParticles2D>("DeathParticles");
        Healthbar = GetNode<ProgressBar>("Healthbar");
        Healthbar.MaxValue = EnemyData.HP;
        Healthbar.Value = EnemyData.HP;
        _hpLabel.Text = $"{EnemyData.HP}/{EnemyData.MaxHP}";
    }

    public void OnEnemySelected(Enemy enemy, int index)
    {
        if (enemy == EnemyData)
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

    public async void Surrender(Enemy enemy)
    {
        if (enemy != EnemyData) return;
        try
        {
            HideHP();
            _animationPlayer.Play("fade_out");
            await ToSignal(_animationPlayer, "animation_finished");
            QueueFree();
        }
        catch (Exception e)
        {
            GD.PushError($"Surrender failed: {e}");
        }
    }

    public async void OnAttackRequested(FighterEventArgs args)
    {
        if (args.Attacker is Ally && args.Defender == EnemyData)
        {
            await PlayEffects(args.Attack as Weapon);
            args.Attacker.AP -= args.Attack.APCost;
            await TakeDamage(args.Attack.ComputeDamage());
            HideHP();

            SignalHub.RaiseFighterAttacked(args.Attacker, args.Defender, args.Attack);
        }
    }

    public async Task PlayEffects(Weapon weapon)
    {
        if (weapon.ID == ItemID.Knife)
        {
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.KnifeSlash);

            var effect = KnifeSlashEffect.Instantiate<AnimatedSprite2D>();
            effect.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(effect);

            await ToSignal(effect, "animation_finished");
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 9.0f);
            effect.QueueFree();
            Bleed();
            Blink();
        }
        else if (weapon.ID == ItemID.BaseballBat)
        {
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.BaseballBatSwing, 1.0f);

            await Game.Instance.Wait(1000);

            var effect = BaseballBatHitEffect.Instantiate<BaseballBatHit>();
            AddChild(effect);

            SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 9.0f);
            await Game.Instance.Wait(200);
            effect.QueueFree();
            Shake();
        }
    }

    public async Task<TalkActionEffect> RespondToTalkAction(Player player, TalkAction action)
    {
        HideHP();
        TalkActionResult result = action.Result;
        SignalHub.Instance.EmitSignal(SignalHub.SignalName.CombatLogUpdateRequested, result.InitialLogEntry);
        await ToSignal(SignalHub.Instance, SignalHub.SignalName.CombatLogUpdated);

        if (Game.Instance.CombatSpeechCheck(player, EnemyData, action.Difficulty))
        {
            await ChatBalloon.PlayMessage($"[shake rate={TextShakeRate} level={TextShakeLevel}]" + result.SuccessBalloonText + "[/shake]", 700);
            SignalHub.Instance.EmitSignal(SignalHub.SignalName.CombatLogUpdateRequested, result.SuccessLogEntry);
            ApplyTalkActionEffect(result.Effect, player);
        }
        else
        {
            await ChatBalloon.PlayMessage($"[shake rate={TextShakeRate} level={TextShakeLevel}]" + result.FailureBalloonText + "[/shake]", 700);
            SignalHub.Instance.EmitSignal(SignalHub.SignalName.CombatLogUpdateRequested, result.FailureLogEntry);
        }

        return result.Effect;
    }

    private void ApplyTalkActionEffect(TalkActionEffect effect, Player player)
    {
        switch (effect)
        {
            case TalkActionEffect.RevealWeakness:
                EnemyData.WeaknessExposed = true;
                break;
            case TalkActionEffect.Surrender:
                SignalHub.Instance.EmitSignal(SignalHub.SignalName.EnemySurrendered, EnemyData);
                break;
            case TalkActionEffect.Death:
                _ = Die();
                break;
            case TalkActionEffect.None:
            default:
                // No effect
                break;
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
        double toValue = Mathf.Clamp(Healthbar.Value - damage, 0, EnemyData.HP);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(Healthbar, "value", toValue, 1.0f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        _hpLabel.Text = $"{Math.Round(toValue)}/{EnemyData.MaxHP}";

        await ToSignal(tween, "finished");
    }

    public async Task Monologue()
    {
        int index = _rng.Next(EnemyData.AttackBalloonText.Length);
        if (index == _prevMonologueIndex)
        {
            // This should prevent repeated dialogue from popping up during fights.
            index = (index + 1) % EnemyData.AttackBalloonText.Length;
        }

        await ChatBalloon.PlayMessage($"[shake rate={TextShakeRate} level={TextShakeLevel}]" + EnemyData.AttackBalloonText[index] + "[/shake]", 700);
        _prevMonologueIndex = index;
    }

    public async Task Introduction()
    {
        await ChatBalloon.PlayMessage($"[shake rate={TextShakeRate} level={TextShakeLevel}]" + EnemyData.IntroBalloon + "[/shake]", 700);
    }

    public async Task Die()
    {
        _animatedSprite2D.Visible = false;
        _animationPlayer.Play("destroy_extras");

        DeathParticles.Restart();
        DeathParticles.Emitting = true;

        // force at least one idle frame so you can actually SEE the particles start
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ChatBalloon.PlayMessage($"[shake rate={TextShakeRate} level={TextShakeLevel}]" + EnemyData.DeathMsgBalloon + "[/shake]", 700, false);

        var timer = GetTree().CreateTimer(0.2);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

    public void Blink()
    {
        _shader.SetShaderParameter("blinking", true);
        _shader.SetShaderParameter("start_time", Time.GetTicksMsec() / 1000.0f);
        GetTree().CreateTimer(BlinkSeconds).Timeout += () =>
        {
            _shader.SetShaderParameter("blinking", false);
        };
    }

    public void Shake()
    {
        _shader.SetShaderParameter("shaking", true);
        _shader.SetShaderParameter("initial_shake_speed", InitialShakeSpeed);
        _shader.SetShaderParameter("shake_intensity", ShakeIntensity);
        _shader.SetShaderParameter("start_time", Time.GetTicksMsec() / 1000.0f);
        GetTree().CreateTimer(ShakeDuration).Timeout += () =>
        {
            _shader.SetShaderParameter("initial_shake_speed", 0.0f);
            _shader.SetShaderParameter("shake_intensity", 0.0f);
            _shader.SetShaderParameter("shaking", false);
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
