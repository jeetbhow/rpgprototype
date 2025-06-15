#nullable enable

using System;

public partial class ChoiceData
{
    public required string Type { get; set; }
    public required string Text { get; set; }
    public string? NextId { get; set; }
    public DialogueNode? Next { get; set; }
}