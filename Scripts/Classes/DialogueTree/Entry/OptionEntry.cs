#nullable enable

using Godot;
using System;

public partial class OptionEntry
{
    public required string Text { get; set; }
    public string? Next { get; set; }
}
