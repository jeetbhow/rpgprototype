using System.Threading.Tasks;
using Godot;
using Signal;

public partial class ChoiceState : StateNode
{
    [Export]
    public UI UI { get; set; }

    [Export]
    public Textbox Textbox { get; set; }

    [Export]
    public StateNode EnabledStateNode { get; set; }

    [Export]
    public StateNode SkillCheckStateNode { get; set; }

    [Export]
    public StateNode DisabledStateNode { get; set; }

    [Export]
    public PackedScene ChoiceButtonScene { get; set; }

    private SoundManager _soundManager;
    private int _index;

    public override void _Ready()
    {
        base._Ready();
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.Pressed)
            return;

        ChoiceContent[] btns = Textbox.GetChoices();
        if (btns == null || btns.Length == 0)
            return;

        btns[_index].HideArrow();

        if (keyEvent.IsActionPressed("MoveDown"))
        {
            _soundManager.PlaySfx(SoundManager.Sfx.Confirm);
            _index = (_index + 1) % btns.Length;
        }
        else if (keyEvent.IsActionPressed("MoveUp"))
        {
            _soundManager.PlaySfx(SoundManager.Sfx.Confirm);
            _index = (_index + btns.Length - 1) % btns.Length;
        }
        else if (keyEvent.IsActionPressed("Accept"))
        {
            OnChoiceSelected(btns[_index]);
        }

        btns[_index].ShowArrow();

        if (btns[_index].Choice is SkillCheck skillCheck)
        {
            UI.SetAndShowTooltip(skillCheck);
        }
        else
        {
            UI.HideTooltip();
        }

        GetViewport().SetInputAsHandled();
    }

    public override async Task Enter()
    {
        _index = 0;
        
        ChoiceNode node = (ChoiceNode)UI.CurrNode;
        Choice[] choices = node.Choices;

        for (int i = 0; i < choices.Length; i++)
        {
            var currChoice = choices[i];

            var btn = (ChoiceContent)ChoiceButtonScene.Instantiate();
            btn.Label.Text = currChoice.Text;
            btn.Choice = currChoice;
            if (i != 0) btn.HideArrow();

            if (currChoice.Type == "skill")
            {
                var skillCheckData = (SkillCheck)currChoice;
                btn.SetColor(SkillManager.GetSkillColor(skillCheckData.Skill.Type));
            }

            Textbox.Choices.AddChoice(btn);
            Textbox.ShowChoices();
        }
    }

    public override async Task Exit()
    {
        Textbox.ClearChoices();
    }
        
    private async void OnChoiceSelected(ChoiceContent btn)
    {
        _soundManager.PlaySfx(SoundManager.Sfx.Confirm);

        await ToSignal(GetTree().CreateTimer(0.2f), Timer.SignalName.Timeout);

        Choice choice = btn.Choice;
        switch (choice.Type)
        {
            case "regular":
                UI.CurrNode = choice.Next;
                SignalHub.Instance.EmitSignal(SignalHub.SignalName.TextboxOptionSelected, UI.CurrNode.Key);
                EmitSignal(SignalName.StateUpdate, EnabledStateNode.Name);
                break;
            case "skill":
                UI.CurrSkillCheck = (SkillCheck)choice;
                EmitSignal(SignalName.StateUpdate, SkillCheckStateNode.Name);
                break;
            case "exit":
                UI.CurrNode = null;
                EmitSignal(SignalName.StateUpdate, DisabledStateNode.Name);
                break;
        }
    }
}


