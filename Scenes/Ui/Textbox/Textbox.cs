using Godot;

public partial class Textbox : PanelContainer
{
    // Exposed in the inspector
    [Export]
    public DialogueTree Tree;

    [Export]
    public RichTextLabel TextLabel;

    [Export]
    public RichTextLabel NameLabel;

    [Export]
    public TextureRect PortraitTexture;

    // Runtime state
    public DialogueNode CurrNode;
}
