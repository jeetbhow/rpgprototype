using Godot;
using System.Threading.Tasks;

using Newtonsoft.Json;

public partial class SkillCheckState : StateNode
{
    [Export] public StateNode WaitingStateNode { get; set; }
    [Export] public Textbox Textbox { get; set; }

    private SoundManager _soundManager;

    public override void _Ready()
    {
        base._Ready();
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
    }

    public override async Task Enter()
    {
        SkillCheckData skill = Textbox.CurrSkillCheck;
        bool result = PerformSkillCheck(skill.SkillId, skill.Difficulty);
        Textbox.ResetText();
        if (result)
        {
            await Textbox.ProcessAndWriteText(".[pause=1000] .[pause=1000] .[pause=1000] You passed!");
            _soundManager.PlaySfx(SoundManager.Sfx.Success);
            Textbox.CurrNode.Next = skill.SuccessNext;
        }
        else
        {
            await Textbox.ProcessAndWriteText(".[pause=1000] .[pause=1000] .[pause=1000] You failed!");
            _soundManager.PlaySfx(SoundManager.Sfx.Fail);
            Textbox.CurrNode.Next = skill.FailNext;
        }
        EmitSignal(SignalName.StateUpdate, WaitingStateNode.Name);
    }

    public static bool PerformSkillCheck(int skillId, int difficulty)
    {
        RandomNumberGenerator rng = new();
        rng.Randomize();
        int roll = rng.RandiRange(1, 20);
        return roll >= difficulty;
    }

    public override async Task Exit()
    { }

}
