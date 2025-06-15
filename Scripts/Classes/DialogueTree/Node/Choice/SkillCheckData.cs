#nullable enable

public partial class SkillCheckData : ChoiceData
{
    public required int SkillId { get; set; }
    public required int Difficulty { get; set; }
    public required string SuccessNextId { get; set; }
    public required string FailNextId { get; set; }
    public DialogueNode? SuccessNext { get; set; }
    public DialogueNode? FailNext { get; set; }
}