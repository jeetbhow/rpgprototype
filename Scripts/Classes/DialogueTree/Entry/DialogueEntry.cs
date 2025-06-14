#nullable enable

public partial class DialogueEntry
{
    public required string Type { get; set; }
    public required string Text { get; set; }
    public string? Name { get; set; }
    public string? Portrait { get; set; }
    public OptionEntry[]? Options { get; set; }
    public string? Next { get; set; }
}
