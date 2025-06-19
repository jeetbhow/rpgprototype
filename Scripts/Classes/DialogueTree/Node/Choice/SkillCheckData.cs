#nullable enable

public partial class SkillCheck : Choice
{
    public required Skill Skill { get; set; }
    public required string SuccessNextId { get; set; }
    public required string FailNextId { get; set; }
    public DialogueNode? SuccessNext { get; set; }
    public DialogueNode? FailNext { get; set; }
}