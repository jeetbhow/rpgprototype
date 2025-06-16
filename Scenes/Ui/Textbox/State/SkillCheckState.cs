using Godot;
using System.Threading.Tasks;

using Newtonsoft.Json;

public partial class SkillCheckState : StateNode
{
    [Export] public Textbox Textbox { get; set; }

    public override async Task Enter()
    {
        SkillCheckData skill = Textbox.CurrSkillCheck;
        bool result = PerformSkillCheck(skill.SkillId, skill.Difficulty);
        if (result)
        {
            // Win
            
        }
        else
        {
            // Lose
        }
    }

    public bool PerformSkillCheck(int skillId, int difficulty)
    {
        RandomNumberGenerator rng = new();
        rng.Randomize();
        int roll = rng.RandiRange(1, 20);
        return roll >= difficulty;
    }

    public override async Task Exit()
    { }

}
