using Godot;

public partial class BattleUI : CanvasLayer
{
    private static readonly string PartyInfoPath = "MarginContainer/PartyInfo";
    private static readonly string CommandTextboxPath = "MarginContainer/BattleInfo/CommandTextbox";
    private static readonly string LogTextboxPath = "MarginContainer/BattleInfo/LogTextbox";

    public HBoxContainer PartyInfoHBox { get; private set; }
    public Textbox CommandTextbox { get; private set; }
    public Textbox LogTextbox { get; private set; }

    public void Init()
    {
        PartyInfoHBox = GetNode<HBoxContainer>(PartyInfoPath);
        CommandTextbox = GetNode<Textbox>(CommandTextboxPath);
        LogTextbox = GetNode<Textbox>(LogTextboxPath);

        LogTextbox.NameLabel.Visible = false;
        LogTextbox.Choices.Visible = false;
        LogTextbox.TextLabel.Visible = true;
        LogTextbox.Visible = true;

        CommandTextbox.Choices.AddChoice("Attack");
        CommandTextbox.Choices.AddChoice("Defend");
        CommandTextbox.Choices.AddChoice("Item");
        CommandTextbox.Choices.AddChoice("Run");
        CommandTextbox.Choices.HideAllArrows();
        CommandTextbox.Choices.ShowArrow(0);
        CommandTextbox.Choices.Visible = true;
        GD.Print("Commands initialized.");
    }
}
