#nullable enable

public partial class SerializedDialogue
{
    public required string Type { get; set; }
    public required string Text { get; set; }
    public string? Name { get; set; }
    public string? Portrait { get; set; }
    public string? Next { get; set; }
}
