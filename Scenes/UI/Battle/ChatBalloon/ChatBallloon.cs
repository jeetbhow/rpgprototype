using Godot;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class ChatBallloon : PanelContainer
{
    private RichTextLabel _textLabel;
    private AudioStreamPlayer _sfx;
    private Timer _textAdvanceTimer;
    private Timer _sfxTimer;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _textLabel = GetNode<RichTextLabel>("MarginContainer/Text");
        _sfx = GetNode<AudioStreamPlayer>("Sfx");
        _textAdvanceTimer = GetNode<Timer>("TextAdvanceTimer");

        _sfxTimer = GetNode<Timer>("SfxTimer");
        _sfxTimer.Timeout += OnSfxTimerTimeout;

    }

    /// <summary>
    /// Play the chat balloon's message. This function assumes that you've already
    /// set the Text property of this ChatBalloon beforehand.
    /// </summary>
    public async Task PlayMessage(string text, bool reset = true)
    {
        Visible = true;

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

            // --- visible text ---------------------------------------------------
            int start = _textLabel.GetTotalCharacterCount();   // counts *visible* chars
            _textLabel.AppendText(part);
            int end = _textLabel.GetTotalCharacterCount();

            await Typewriter(start, end);

            _textLabel.VisibleCharacters = end;
        }

        await Game.Instance.Wait(500);
        if (reset)
        {
            _textLabel.Text = "";
            _textLabel.VisibleCharacters = 0;
            Visible = false;
        }
    }

    private async Task Typewriter(int start, int end)
    {
        _textAdvanceTimer.Start();
        _sfxTimer.Start();

        int curr = start;
        while (_textLabel.VisibleCharacters < end)
        {
            _textLabel.VisibleCharacters = ++curr;
            await ToSignal(_textAdvanceTimer, Timer.SignalName.Timeout);
            //await ToSignal(GetTree().CreateTimer(0.02f), Timer.SignalName.Timeout);
        }

        _sfxTimer.Stop();
        _textAdvanceTimer.Stop();
    }

    private void OnSfxTimerTimeout()
    {
        _sfx.Play();
    }
}
