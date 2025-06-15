using Godot;
using System.Collections.Generic;

public partial class Textbox : PanelContainer
{
    [Export] public DialogueTree Tree { get; set; }
    [Export] public RichTextLabel TextLabel { get; set; }
    [Export] public RichTextLabel NameLabel { get; set; }
    [Export] public TextureRect Portrait { get; set; }
    [Export] public VBoxContainer ChoiceContainer { get; set; }
    [Export] public Timer TextAdvanceTimer;
    [Export] public Timer SfxTimer;
    [Export] public AudioStreamPlayer Sfx;

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
        "res://Scenes/UI/Textbox/Button/TextboxChoice.tscn"
    );

    public override void _Ready()
    {
        base._Ready();
        nameLabelSize = NameLabel.CustomMinimumSize;
        textLabelSize = TextLabel.CustomMinimumSize;
        portraitSize = Portrait.CustomMinimumSize;
    }

    public void ResetTextboxState()
    {
        TextLabel.VisibleCharacters = 0;
        NameLabel.CustomMinimumSize = nameLabelSize;
        TextLabel.CustomMinimumSize = textLabelSize;
        Portrait.CustomMinimumSize = portraitSize;
    }

    public void StartTimers()
    {
        TextAdvanceTimer.Start();
        SfxTimer.Start();
    }

    public void LoadNextNode()
    {
        if (CurrNode == null)
        {
            GD.PrintErr("CurrNode is null. Cannot load the next node.");
            return;
        }

        TextLabel.Text = CurrNode.Text ?? string.Empty;
        NameLabel.Text = CurrNode.Name ?? string.Empty;
        NameLabel.CustomMinimumSize = string.IsNullOrEmpty(NameLabel.Text) ? Vector2.Zero : nameLabelSize;

        if (!string.IsNullOrEmpty(CurrNode.Portrait) && portraits.TryGetValue(CurrNode.Portrait, out var tex))
        {
            Portrait.Texture = tex;
            Portrait.CustomMinimumSize = portraitSize;
        }
        else
        {
            Portrait.Texture = null;
            Portrait.CustomMinimumSize = Vector2.Zero;
            TextLabel.CustomMinimumSize += portraitSize;
        }
    }

    public void PopulateChoiceContainer()
    {
        ChoiceNode node = (ChoiceNode)CurrNode;
        foreach (ChoiceData choice in node.ChoiceData)
        {
            var choiceScene = TextboxChoiceScene.Instantiate() as TextboxChoice;
            ChoiceContainer.AddChild(choiceScene);
            choiceScene.Label.Text = choice.Text;
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
}
