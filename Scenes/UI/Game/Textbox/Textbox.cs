using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Godot;

public partial class Textbox : PanelContainer
{
    [Export] public DialogueTree Tree { get; set; }
    [Export] public RichTextLabel TextLabel { get; set; }
    [Export] public RichTextLabel NameLabel { get; set; }
    [Export] public ChoiceList Choices { get; set; }
    [Export] public Timer SfxTimer { get; set; }
    [Export] public AudioStreamPlayer Sfx { get; set; }
    
    public bool SkipRequested { get; set; }
    public string TextBuffer { get; set; }      // Contains the entire text buffer with patterns removed.

    private SoundManager _soundManager;

    public override void _Ready()
    {
        // This timer plays the typewriter sfx.
        SfxTimer.Timeout += OnSfxTimeout;
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
    }

    public ChoiceContent[] GetChoices()
    {
        return Choices.GetChoices();
    }

    public void HideName()
    {
        NameLabel.Visible = false;
    }

    public void ShowName(string name)
    {
        NameLabel.Text = name;
        NameLabel.Visible = true;
    }

    public void ShowChoices()
    {
        Choices.Visible = true;
    }

    public void AppendText(string text)
    {
        TextLabel.AppendText(text);
    }

    public void SetText(string text)
    {
        TextLabel.Text = text;
    }

    public void OnSfxTimeout()
    {
        Sfx.Play();
    }

    public void SetVisibleChars(int value)
    {
        TextLabel.VisibleCharacters = value;
    }

    public void ClearChoices()
    {
        Choices.Clear();
        Choices.Visible = false;
        TextLabel.VisibleCharacters = 0;
    }

    public async Task ProcessAndWriteText(string text)
    {
        const string pattern = @"(\[\w+=\d+\])";

        TextBuffer = Regex.Replace(text, pattern, "");

        string[] parts = Regex.Split(text, pattern);
        int visibleChars = 0;
        foreach (var part in parts)
        {
            if (SkipRequested) break;

            // Parse the command
            if (part.StartsWith('[') && part.EndsWith(']'))
            {
                string inner = part[1..^1];
                string[] tokens = inner.Split('=');
                string command = tokens[0];
                string value = tokens[1];

                switch (command)
                {
                    case "pause":
                        await ToSignal(
                            GetTree().CreateTimer(float.Parse(value) / 1000),       // Convert ms to s.
                            Timer.SignalName.Timeout
                        );
                        break;
                }
                continue;
            }

            // Render the actual text with the scrolling effect.
            int start = visibleChars;
            int end = start + part.Length;
            AppendText(part);

            await Typewriter(start, end);

            visibleChars = end;
        }
    }

    public async Task Typewriter(int start, int end)
    {
        SfxTimer.Start();

        int curr = start;
        while (curr < end)
        {
            SetVisibleChars(++curr);
            await ToSignal(GetTree().CreateTimer(0.02f), Timer.SignalName.Timeout);
            if (SkipRequested) return;
        }

        SfxTimer.Stop();
    }
}
