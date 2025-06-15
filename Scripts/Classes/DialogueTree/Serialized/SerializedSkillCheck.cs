using System;

public partial class SerializedSkillCheck : SerializedChoice
{
    public required int SkillId { get; set; }
    public required int Difficulty { get; set; }
    public required string Success { get; set; }
    public required string Failure { get; set; }
}