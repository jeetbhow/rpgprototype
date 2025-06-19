using Godot;

public partial class Textbox : PanelContainer
{
    [Export] public DialogueTree Tree { get; set; }
    [Export] public RichTextLabel TextLabel { get; set; }
    [Export] public RichTextLabel NameLabel { get; set; }
    [Export] public ChoiceList Choices { get; set; }
    [Export] public Timer SfxTimer { get; set; }
    [Export] public AudioStreamPlayer Sfx { get; set; }

    private SoundManager _soundManager;

    public override void _Ready()
    {
        // This timer plays the typewriter sfx.
        SfxTimer.Timeout += OnSfxTimeout;
        Choices.Visible = false;
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
    }

    public ChoiceButton[] GetChoiceButtons()
    {
        return Choices.GetChoiceButtons();
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

    public void AddChoiceBtn(ChoiceButton btn)
    {
        Choices.AddChild(btn);
    }

    public void ClearChoiceButtons()
    {
        foreach (Node choice in Choices.GetChildren())
        {
            choice.QueueFree();
        }
        Choices.Visible = false;
        TextLabel.VisibleCharacters = 0;
    }

    public void OnSfxTimeout()
    {
        Sfx.Play();
    }

    public void SetVisibleChars(int value)
    {
        TextLabel.VisibleCharacters = value;
    }
}
