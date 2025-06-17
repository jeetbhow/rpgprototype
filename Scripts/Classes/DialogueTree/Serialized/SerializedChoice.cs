#nullable enable

using Godot;

public partial class SerializedChoice : Resource
{
    public required string Type { get; set; }
    public required string Text { get; set; }
    public required string? Next { get; set; }
    public SerializedSkillCheck? SkillCheck { get; set; }
}
