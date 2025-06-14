#nullable enable

public partial class Option
{
    public required string Text { get; set; }
    public required string NextId { get; set; }
    public DialogueNode? Next { get; set; }
}
