using System.Collections.Generic;
using System.Linq;
using Godot;

using Combat;

public partial class BattleUI : CanvasLayer
{
    private string[] CommandNames = ["Attack", "Defend", "Talk", "Item", "Run", "End Turn"];

    private const string PartyInfoPath = "MarginContainer/PartyInfo";
    private const string CommandTextboxPath = "MarginContainer/BattleInfo/Commands";
    private const string LogPath = "MarginContainer/BattleInfo/Log";
    private const string DiceRollsPath = "MarginContainer/DiceRolls";

    private HBoxContainer PartyInfoHBox;

    public Textbox Commands { get; private set; }
    public Log Log { get; private set; }
    public VBoxContainer DiceRolls { get; set; }
    public VBoxContainer TurnQueue { get; set; }

    [Export] public PackedScene TurnQueuePanelScene { get; set; }
    [Export] public PackedScene DiceRollInfoScene { get; set; }


    public override void _Ready()
    {
        PartyInfoHBox = GetNode<HBoxContainer>(PartyInfoPath);
        Commands = GetNode<Textbox>(CommandTextboxPath);
        Log = GetNode<Log>(LogPath);
        DiceRolls = GetNode<VBoxContainer>(DiceRollsPath);
        TurnQueue = GetNode<VBoxContainer>("MarginContainer/TurnQueue");

        Commands.Choices.Visible = true;

        ShowPlayerCommands(0);
    }

    /// <summary>
    /// Slides every panel in <see cref="TurnQueue"/> up one slot and (optionally)
    /// fades the departing panel, then frees it when the tween finishes.
    /// </summary>
    /// <param name="duration">Seconds the slide should take.</param>
    public async void AdvanceTurnQueue(float duration = 0.25f, float offscreenPad = 32f)
    {
        if (TurnQueue.GetChildCount() == 0)
            return;

        var departing = TurnQueue.GetChild<Control>(0);

        // How far the remaining panels need to shift vertically
        var shiftY = departing.Size.Y + TurnQueue.GetThemeConstant("vseparation");

        var tween = GetTree().CreateTween();

        var start = departing.GlobalPosition;
        var end = new Vector2(
            -departing.Size.X - offscreenPad,   // x: safely past the left edge
            start.Y                              // y: unchanged
        );

        tween.TweenProperty(departing, "global_position", end, duration * 0.8f)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.In);

        foreach (Control child in TurnQueue.GetChildren().Cast<Control>().Skip(1))
        {
            var cStart = child.GlobalPosition;
            var cEnd = cStart - new Vector2(0, shiftY);

            tween.TweenProperty(child, "global_position", cEnd, duration)
                 .SetTrans(Tween.TransitionType.Cubic)
                 .SetEase(Tween.EaseType.Out);
        }

        tween.TweenCallback(Callable.From(() =>
        {
            TurnQueue.RemoveChild(departing);
            departing.QueueFree();
        }));

        await ToSignal(tween, "finished");
    }


    public void AddTQPanel(Fighter f)
    {
        var p = TurnQueuePanelScene.Instantiate<TurnQueuePanel>();
        p.Fighter = f;
        TurnQueue.AddChild(p);
    }

    public void SetTurnQueue(Queue<Fighter> q)
    {
        foreach (var pnl in TurnQueue.GetChildren().Cast<TurnQueuePanel>())
        {
            TurnQueue.RemoveChild(pnl);
            pnl.QueueFree();
        }

        foreach (var f in q)
        {
            GD.Print(f.Name);
            AddTQPanel(f);
        }
    }

    public TurnQueuePanel TQPeek()
    {
        return TurnQueue.GetChild<TurnQueuePanel>(0);
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
            panel.PartyMember.AP = panel.PartyMember.MaxAP;
        }
    }

    public void CreateDiceRollInfo(Fighter fighter, int d1, int d2, SkillType skillType, int bonus)
    {
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
