#nullable enable

using Godot;

public partial class Choice : Resource
{
    public required string Type { get; set; }
    public required string Text { get; set; }
    public string? NextId { get; set; }
    public DialogueNode? Next { get; set; }
}