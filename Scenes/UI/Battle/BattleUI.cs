using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class BattleUI : CanvasLayer
{
    private string[] CommandNames = ["Attack", "Defend", "Talk", "Item", "Run", "End Turn"];

    private const string PartyInfoPath = "MarginContainer/PartyInfo";
    private const string CommandTextboxPath = "MarginContainer/BattleInfo/CommandTextbox";
    private const string LogPath = "MarginContainer/BattleInfo/Log";
    private const string DiceRollsPath = "MarginContainer/DiceRolls";

    private HBoxContainer PartyInfoHBox;
    public Textbox Commands { get; private set; }
    public Log Log { get; private set; }
    public VBoxContainer DiceRolls { get; set; }

    [Export] public Battle Battle { get; set; }
    [Export] public PackedScene DiceRollInfoScene { get; set; }

    public void Init()
    {
        PartyInfoHBox = GetNode<HBoxContainer>(PartyInfoPath);
        Commands = GetNode<Textbox>(CommandTextboxPath);
        Log = GetNode<Log>(LogPath);
        DiceRolls = GetNode<VBoxContainer>(DiceRollsPath);
        Commands.Choices.Visible = true;

        ShowPlayerCommands(0);
    }

    public void ShowPlayerCommands(int initialIndex)
    {
        Commands.Choices.Clear();
        foreach (var command in CommandNames)
        {
            Commands.Choices.AddChoice(command);
        }
        Commands.Choices.HideAllArrows();
        Commands.Choices.ShowArrow(initialIndex);
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

    public void CreateDiceRollInfo(Fighter fighter, int d1, int d2, SkillType skillType, int bonus)
    {
        GD.Print("Signal Handler Entered");
        DiceRollInfo diceRollInfo = (DiceRollInfo)DiceRollInfoScene.Instantiate();
        DiceRolls.AddChild(diceRollInfo);

        diceRollInfo.Dice1 = d1;
        diceRollInfo.Dice2 = d2;
        diceRollInfo.Portrait = fighter.Portrait;
        diceRollInfo.SkillType = skillType;
        diceRollInfo.Bonus = bonus;
        diceRollInfo.Show();
    }
}
