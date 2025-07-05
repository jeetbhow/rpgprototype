using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class BattleUI : CanvasLayer
{
    private string[] Commands = ["Attack", "Defend", "Talk", "Item", "Run", "End Turn"];

    private const string PartyInfoPath = "MarginContainer/PartyInfo";
    private const string CommandTextboxPath = "MarginContainer/BattleInfo/CommandTextbox";
    private const string LogPath = "MarginContainer/BattleInfo/Log";
    private const string TurnQueuePath = "MarginContainer/TurnQueue";

    private HBoxContainer PartyInfoHBox;
    public Textbox CommandTextbox { get; private set; }
    public Log Log { get; private set; }
    public VBoxContainer TurnQueueVBox { get; set; }

    [Export] public PackedScene TurnQueuePanelScene { get; set; }

    public void Init()
    {
        PartyInfoHBox = GetNode<HBoxContainer>(PartyInfoPath);
        CommandTextbox = GetNode<Textbox>(CommandTextboxPath);
        Log = GetNode<Log>(LogPath);
        TurnQueueVBox = GetNode<VBoxContainer>(TurnQueuePath);

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

    public async Task SwapTurnQueuePanels(TurnQueuePanel p1, TurnQueuePanel p2, int a, int b)
    {
        await p1.RemoveSmoothly();
        await p2.RemoveSmoothly();
        TurnQueueVBox.MoveChild(p1, b);
        TurnQueueVBox.MoveChild(p2, a);
        await p1.AppearSmoothly();
        await p2.AppearSmoothly();
    }

    public void AddTurnQueuePanel(Fighter fighter, Texture2D portrait)
    {
        var panel = TurnQueuePanelScene.Instantiate<TurnQueuePanel>();
        panel.Fighter = fighter;
        panel.Portrait = portrait;
        TurnQueueVBox.AddChild(panel);
    }

    public async Task RemoveTurnQueuePanel(Fighter fighter)
    {
        int index = FindTurnQueuePanel(fighter);
        TurnQueuePanel panel = GetTurnQueuePanel(index);
        await panel.RemoveSmoothly();
        TurnQueueVBox.RemoveChild(panel);
        panel.QueueFree();
    }

    public TurnQueuePanel GetTurnQueuePanel(int index)
    {
        if (index < 0 || index >= TurnQueueVBox.GetChildCount())
        {
            GD.PrintErr($"Invalid index {index} for TurnQueueVBox.");
            return null;
        }
        return TurnQueueVBox.GetChild<TurnQueuePanel>(index);
    }

    public int FindTurnQueuePanel(Fighter fighter)
    {
        for (int i = 0; i < TurnQueueVBox.GetChildCount(); i++)
        {
            TurnQueuePanel panel = TurnQueueVBox.GetChild<TurnQueuePanel>(i);
            if (panel.Fighter == fighter)
            {
                return i;
            }
        }
        GD.PrintErr($"No TurnQueuePanel found for Fighter: {fighter.Name}");
        return -1;
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
