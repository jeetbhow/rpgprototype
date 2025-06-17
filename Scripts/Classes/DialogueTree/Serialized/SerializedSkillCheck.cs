using Godot;

public partial class SerializedSkillCheck : SerializedChoice
{
    public required string SkillName { get; set; }
    public required int Difficulty { get; set; }
    public required string Success { get; set; }
    public required string Failure { get; set; }
}