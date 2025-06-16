using Godot;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Textbox : Control
{

    [Export] public DialogueTree Tree { get; set; }
    [Export] public RichTextLabel TextLabel { get; set; }
    [Export] public RichTextLabel NameLabel { get; set; }
    [Export] public Portrait Portrait { get; set; }
    [Export] public VBoxContainer ChoiceContainer { get; set; }
    [Export] public Timer SfxTimer { get; set; }
    [Export] public AudioStreamPlayer Sfx { get; set; }

    public DialogueNode CurrNode { get; set; }
    public SkillCheckData CurrSkillCheck { get; set; }
    public bool SkipRequested { get; set; }
    public string  FullText { get; set; }

    // We store the sizes of the UI elements so that we can restore them later.
    private Vector2 _nameLabelSize;
    private Vector2 _textLabelSize;
    private Vector2 _portraitSize;

    private readonly Dictionary<string, Texture2D> portraits = new()
    {
        { "Jeet", (Texture2D)GD.Load("res://assets/characters/jeet/jeet-face.png") }
    };

    private static readonly PackedScene TextboxChoiceScene = GD.Load<PackedScene>(
        "res://Scenes/UI/Textbox/Choice/TextboxChoice.tscn"
    );

    public override void _Ready()
    {
        base._Ready();

        // This timer plays the typewriter sfx.
        SfxTimer.Timeout += OnSfxTimeout;

        _nameLabelSize = NameLabel.CustomMinimumSize;
        _textLabelSize = TextLabel.CustomMinimumSize;
    }

    public void ResetTextboxState()
    {
        SkipRequested = false;
        TextLabel.Text = "";
        TextLabel.VisibleCharacters = 0;
        NameLabel.CustomMinimumSize = _nameLabelSize;
        TextLabel.CustomMinimumSize = _textLabelSize;
    }

    public void ResetText()
    {   
        SkipRequested = false;
        TextLabel.Text = "";
        TextLabel.VisibleCharacters = 0;
    }

    public string LoadNextNode()
    {
        if (CurrNode == null)
        {
            GD.PrintErr("CurrNode is null. Cannot load the next node.");
            return null;
        }

        NameLabel.Text = CurrNode.Name ?? string.Empty;
        NameLabel.CustomMinimumSize = string.IsNullOrEmpty(NameLabel.Text) ? Vector2.Zero : _nameLabelSize;

        if (!string.IsNullOrEmpty(CurrNode.Portrait) && portraits.TryGetValue(CurrNode.Portrait, out var tex))
        {
            Portrait.SetTexture(tex);
            Portrait.Show();
        }
        else
        {
            Portrait.SetTexture(null);
            Portrait.Hide();
            TextLabel.CustomMinimumSize += Portrait.GetSize();
        }

        return CurrNode.Text;
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

    public async Task ProcessAndWriteText(string text)
    {
        string pattern = @"(\[\w+=\d+\])";
        FullText = Regex.Replace(text, pattern, "");
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
            TextLabel.Text += part;

            await Typewriter(start, end);

            visibleChars = end;
        }
    }

    public void PopulateChoiceContainer()
    {
        ChoiceNode node = (ChoiceNode)CurrNode;
        foreach (ChoiceData choice in node.ChoiceData)
        {
            var choiceScene = TextboxChoiceScene.Instantiate() as TextboxChoice;
            ChoiceContainer.AddChild(choiceScene);
            choiceScene.RichTextLabel.Text = choice.Text;
        }
    }

    public void CleanUpChoiceContainer()
    {
        foreach (Node choice in ChoiceContainer.GetChildren())
        {
            choice.QueueFree();
        }
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
