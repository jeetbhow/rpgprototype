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
        Textbox.ResetText();

        bool result = SkillManager.PerformSkillCheck(Textbox.PlayerData, Textbox.CurrSkillCheck);
        if (result)
        {
            await Textbox.ProcessAndWriteText(".[pause=1000] .[pause=1000] .[pause=1000] You passed!");
            _soundManager.PlaySfx(SoundManager.Sfx.Success);
            Textbox.CurrNode.Next = Textbox.CurrSkillCheck.SuccessNext;
        }
        else
        {
            await Textbox.ProcessAndWriteText(".[pause=1000] .[pause=1000] .[pause=1000] You failed!");
            _soundManager.PlaySfx(SoundManager.Sfx.Fail);
            Textbox.CurrNode.Next = Textbox.CurrSkillCheck.FailNext;
        }
        EmitSignal(SignalName.StateUpdate, WaitingStateNode.Name);
    }

    public override async Task Exit()
    { }

}
