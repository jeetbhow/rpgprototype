using Godot;

public partial class ColorRect : Godot.ColorRect
{

    private Tween tween;

    public override void _Ready()
    {
        base._Ready();
        var signalHub = GetNode<SignalHub>("/root/SignalHub");
        signalHub.SkillCheckFailed += () => FlashScreen(new Color(1.0f, 0.0f, 0.0f));
        signalHub.SkillCheckPassed += () => FlashScreen(new Color(0.0f, 1.0f, 0.0f));
    }

    public async void FlashScreen(Color color, float duration = 0.3f)
    {
        ShaderMaterial mat = (ShaderMaterial)Material;
        mat.SetShaderParameter("flash_color", color);
        mat.SetShaderParameter("flash_strength", 1.0f);

        tween?.Kill();

        tween = CreateTween();
        tween.TweenProperty(mat, "shader_parameter/flash_strength", 0.0f, duration);
    }

}
