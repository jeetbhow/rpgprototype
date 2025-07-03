using Godot;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

[GlobalClass]
public partial class Log : PanelContainer
{
    private Timer _sfxTimer;
    private AudioStreamPlayer _sfxPlayer;
    private int _visibleChars = 0;

    public RichTextLabel TextLabel { get; private set; }

    public override void _Ready()
    {
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
        // Wait a little before logging the next line.
        await Task.Delay(200);

        text += "\n";
        const string pattern = @"(\[\w+=\d+\])";

        string[] parts = Regex.Split(text, pattern);
        foreach (var part in parts)
        {            
            await ProcessText(part);

            int start = _visibleChars;
            int end = start + part.Length;
            TextLabel.AppendText(part);

            await Typewriter(start, end);

            _visibleChars = end;
        }
    }

    private async Task Typewriter(int start, int end)
    {
        _sfxTimer.Start();

        int curr = start;
        while (curr < end)
        {
            TextLabel.VisibleCharacters = ++curr;
            await ToSignal(GetTree().CreateTimer(0.02f), Timer.SignalName.Timeout);
        }

        _sfxTimer.Stop();
    }

    private async Task ProcessText(string text)
    {
        if (text.StartsWith('[') && text.EndsWith(']'))
        {
            string inner = text[1..^1];
            string[] tokens = inner.Split('=');
            string command = tokens[0];
            string value = tokens[1];

            switch (command)
            {
                case "pause":
                    await ToSignal(
                        GetTree().CreateTimer(float.Parse(value) / 1000),
                        Timer.SignalName.Timeout
                    );
                    break;
            }
        }
    }
}
