#nullable enable

public partial class DialogueNode
{
    public required string Key { get; set; }
    public required string Type { get; set; }
    public required string Text { get; set; }
    public string? Name { get; set; }
    public string? Portrait { get; set; }
    public DialogueNode? Next { get; set; }
}