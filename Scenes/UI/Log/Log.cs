using Godot;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

using Signal;

[GlobalClass]
public partial class Log : PanelContainer
{
    private Timer _sfxTimer;
    private AudioStreamPlayer _sfxPlayer;
    private int _visibleChars = 0;

    public RichTextLabel TextLabel { get; private set; }

    public override void _Ready()
    {
        SignalHub.Instance.CombatLogUpdateRequested += async text => await AppendLine(text);
    
        TextLabel = GetNode<RichTextLabel>("MarginContainer/RichTextLabel");
        _sfxTimer = GetNode<Timer>("SfxTimer");
        _sfxPlayer = GetNode<AudioStreamPlayer>("SfxPlayer");
        _sfxTimer.Timeout += OnSfxTimeout;
    }

    private void OnSfxTimeout()
    {
        _sfxPlayer.Play();
    }

    public async Task AppendLine(string text)
    {
        await Game.Instance.Wait(200);
        const string pattern = @"(\[[^\]]+\])";
        string[] parts = Regex.Split(text + "\n", pattern);

        foreach (var part in parts)
        {
            if (part.StartsWith("[pause="))
            {
                var ms = float.Parse(part[8..^1], CultureInfo.InvariantCulture);
                await ToSignal(GetTree().CreateTimer(ms / 1000f), Timer.SignalName.Timeout);
                continue;
            }

            int start = TextLabel.GetTotalCharacterCount();
            TextLabel.AppendText(part);
            int end = TextLabel.GetTotalCharacterCount();

            await Typewriter(start, end);

            _visibleChars = end;
        }

        SignalHub.Instance.EmitSignal(SignalHub.SignalName.CombatLogUpdated);
    }

    private async Task Typewriter(int start, int end)
    {
        _sfxTimer.Start();

        int curr = start;
        while (TextLabel.VisibleCharacters < end)
        {
            TextLabel.VisibleCharacters = ++curr;
            await ToSignal(GetTree().CreateTimer(0.02f), Timer.SignalName.Timeout);
        }

        _sfxTimer.Stop();
    }
}
