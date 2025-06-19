using System.Threading.Tasks;
using Godot;

public partial class ChoiceState : StateNode
{
    [Export] UI UI { get; set; }
    [Export] Textbox Textbox { get; set; }
    [Export] StateNode EnabledStateNode { get; set; }
    [Export] StateNode SkillCheckStateNode { get; set; }
    [Export] StateNode DisabledStateNode { get; set; }
    [Export] PackedScene ChoiceButtonScene { get; set; }

    private SoundManager _soundManager;
    private EventBus _eventBus;
    private int _index;

    public override void _Ready()
    {
        base._Ready();
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
        _eventBus = GetNode<EventBus>(EventBus.Path);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.Pressed)
            return;

        ChoiceButton[] btns = Textbox.GetChoiceButtons();
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

            var btn = (ChoiceButton)ChoiceButtonScene.Instantiate();
            btn.Label.Text = currChoice.Text;
            btn.Choice = currChoice;
            if (i != 0) btn.HideArrow();

            if (currChoice.Type == "skill")
            {
                var skillCheckData = (SkillCheck)currChoice;
                btn.SetColor(SkillManager.GetSkillColor(skillCheckData.Skill.Type));
            }

            Textbox.AddChoiceBtn(btn);
            Textbox.ShowChoices();
        }
    }

    public override async Task Exit()
    {
        Textbox.ClearChoiceButtons();
    }
        
    private async void OnChoiceSelected(ChoiceButton btn)
    {
        _soundManager.PlaySfx(SoundManager.Sfx.Confirm);

        await ToSignal(GetTree().CreateTimer(0.2f), Timer.SignalName.Timeout);

        Choice choice = btn.Choice;
        switch (choice.Type)
        {
            case "regular":
                UI.CurrNode = choice.Next;
                _eventBus.EmitSignal(EventBus.SignalName.TextboxOptionSelected, UI.CurrNode.Key);
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


