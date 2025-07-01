using Godot;
using System.Threading.Tasks;

public partial class SkillCheckState : StateNode
{
    [Export] public UI UI { get; set; }
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
        UI.Reset();
        
        bool result = SkillManager.PerformSkillCheck(UI.PlayerData, UI.CurrSkillCheck);
        if (result)
        {
            await UI.Textbox.ProcessAndWriteText(".[pause=1000] .[pause=1000] .[pause=1000] You passed!");
            _soundManager.PlaySfx(SoundManager.Sfx.Success);
            UI.CurrNode.Next = UI.CurrSkillCheck.SuccessNext;
        }
        else
        {
            await UI.Textbox.ProcessAndWriteText(".[pause=1000] .[pause=1000] .[pause=1000] You failed!");
            _soundManager.PlaySfx(SoundManager.Sfx.Fail);
            UI.CurrNode.Next = UI.CurrSkillCheck.FailNext;
        }

        EmitSignal(SignalName.StateUpdate, WaitingStateNode.Name);
    }

    public override async Task Exit()
    { }

}
