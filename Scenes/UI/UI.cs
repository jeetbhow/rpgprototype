using Godot;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class UI : Control
{
    // For testing purposes I've exposed this DialogueTree field to the editor.
    [Export] public DialogueTree DialogueTree { get; set; }
    [Export] public Textbox Textbox { get; set; }
    [Export] public Portrait Portrait { get; set; }
    [Export] public ChoiceTooltip Tooltip { get; set; }
    [Export] public PlayerData PlayerData { get; set; }

    public DialogueNode CurrNode { get; set; }
    public SkillCheck CurrSkillCheck { get; set; }

    private SoundManager _soundManager;
    private EventBus _eventBus;
    private readonly Dictionary<string, Texture2D> _portraits = new()
    {
        { "Jeet", (Texture2D)GD.Load("res://assets/characters/jeet/jeet-face.png") }
    };

    public override void _Ready()
    {
        Tooltip.Hide();
        _soundManager = GetNode<SoundManager>(SoundManager.Path);
        _eventBus = GetNode<EventBus>(EventBus.Path);
    }

    public void Stop()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    public void Start()
    {
        GetTree().Paused = true;
        Visible = true;
    }

    public void ShowPortrait(Texture2D tex)
    {
        Portrait.SetTexture(tex);
        Portrait.Visible = true;
    }

    public void HidePortrait()
    {
        Portrait.Visible = false;
    }

    public void LoadDialogue(DialogueTree tree)
    {
        DialogueTree = tree;
        CurrNode = tree.Root;
    }

    public async Task WriteText()
    {
        Reset();
        await Textbox.ProcessAndWriteText(LoadCurrNode());
    }

    public void Reset()
    {
        Textbox.SkipRequested = false;
        Textbox.TextLabel.Text = "";
        Textbox.TextLabel.VisibleCharacters = 0;
    }

    public void Skip()
    {
        Textbox.SkipRequested = true;
        Textbox.SetText(Textbox.TextBuffer);
        Textbox.SetVisibleChars(Textbox.TextBuffer.Length);
        Textbox.SfxTimer.Stop();
    }

    public string LoadCurrNode()
    {
        if (CurrNode == null) return null;

        if (string.IsNullOrEmpty(CurrNode.Name))
            Textbox.HideName();
        else Textbox.ShowName(CurrNode.Name);

        if (string.IsNullOrEmpty(CurrNode.Portrait))
            HidePortrait();
        else ShowPortrait(_portraits[CurrNode.Portrait]);
        return CurrNode.Text;
    }

    public void SetAndShowTooltip(SkillCheck skillCheck)
    {
        SkillType type = skillCheck.Skill.Type;
        int skillCheckDifficulty = skillCheck.Skill.Value;
        int playerSkillValue = PlayerData.GetSkillValue(type);

        Tooltip.SkillColor = SkillManager.GetSkillColor(type);
        Tooltip.SkillName = Skill.StringFromType(skillCheck.Skill.Type);
        Tooltip.PlayerSkillValue = playerSkillValue;
        Tooltip.Difficulty = skillCheckDifficulty;
        Tooltip.Probability = SkillManager.GetSuccessChance(PlayerData, skillCheck);
        Tooltip.Category = SkillManager.GetProbabilityCategory(Tooltip.Probability);

        Tooltip.Update();
        Tooltip.Show();
    }

    public void HideTooltip()
    {
        Tooltip.Hide();
    }
}
