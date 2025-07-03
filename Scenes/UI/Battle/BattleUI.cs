using System.Linq;
using Godot;

public partial class BattleUI : CanvasLayer
{
    private string[] Commands = ["Attack", "Defend", "Talk", "Item", "Run", "End Turn"];

    private const string PartyInfoPath = "MarginContainer/PartyInfo";
    private const string CommandTextboxPath = "MarginContainer/BattleInfo/CommandTextbox";
    private const string LogPath = "MarginContainer/BattleInfo/Log";

    private HBoxContainer PartyInfoHBox;

    public Textbox CommandTextbox { get; private set; }
    public Log Log { get; private set; }

    public void Init()
    {
        PartyInfoHBox = GetNode<HBoxContainer>(PartyInfoPath);
        CommandTextbox = GetNode<Textbox>(CommandTextboxPath);
        Log = GetNode<Log>(LogPath);

        CommandTextbox.Choices.Visible = true;

        ShowPlayerCommands(0);
    }

    public void ShowPlayerCommands(int initialIndex)
    {
        CommandTextbox.Choices.Clear();
        foreach (var command in Commands)
        {
            CommandTextbox.Choices.AddChoice(command);
        }
        CommandTextbox.Choices.HideAllArrows();
        CommandTextbox.Choices.ShowArrow(initialIndex);
    }

    public void AddPartyInfoPanel(PartyInfoPanel panel)
    {
        PartyInfoHBox.AddChild(panel);
    }

    /// <summary>
    /// Get the PartyInfoPanel at the specified index.
    /// </summary>
    /// <param name="index">The index of the PartyInfoPanel.</param>
    /// <returns>The <see cref="PartyInfoPanel"/> at the specified index.</returns>
    public PartyInfoPanel GetPartyInfoPanel(int index)
    {
        if (index < 0 || index >= PartyInfoHBox.GetChildCount())
        {
            GD.PrintErr($"Invalid index {index} for PartyInfoHBox.");
            return null;
        }
        return PartyInfoHBox.GetChild<PartyInfoPanel>(index);
    }

    /// <summary>
    /// Reset the Action Points (AP) of all PartyInfoPanels to their maximum values.
    /// This is typically called at the start of a new turn in battle.
    /// </summary>
    public void ResetAP()
    {
        foreach (PartyInfoPanel panel in PartyInfoHBox.GetChildren().Cast<PartyInfoPanel>())
        {
            panel.AP = panel.MaxAP;
        }
    }
}
