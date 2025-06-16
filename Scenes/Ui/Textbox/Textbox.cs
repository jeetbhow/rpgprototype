using Godot;
using System.Collections.Generic;

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

    // We store the sizes of the UI elements so that we can restore them later.
    private Vector2 nameLabelSize;
    private Vector2 textLabelSize;
    private Vector2 portraitSize;

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

        nameLabelSize = NameLabel.CustomMinimumSize;
        textLabelSize = TextLabel.CustomMinimumSize;
    }

    public void ResetTextboxState()
    {
        TextLabel.VisibleCharacters = 0;
        NameLabel.CustomMinimumSize = nameLabelSize;
        TextLabel.CustomMinimumSize = textLabelSize;
    }

    public string LoadNextNode()
    {
        if (CurrNode == null)
        {
            GD.PrintErr("CurrNode is null. Cannot load the next node.");
            return null;
        }

        NameLabel.Text = CurrNode.Name ?? string.Empty;
        NameLabel.CustomMinimumSize = string.IsNullOrEmpty(NameLabel.Text) ? Vector2.Zero : nameLabelSize;

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
        RandomNumberGenerator rng = new();
        float randf = rng.RandfRange(0.85f, 1.15f);
        Sfx.PitchScale = randf;
        Sfx.Play();
    }

    public void SetVisibleChars(int value)
    {
        TextLabel.VisibleCharacters = value;
    }
}
