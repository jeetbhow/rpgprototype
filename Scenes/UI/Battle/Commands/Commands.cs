using Godot;

public partial class Commands : PanelContainer
{
    private static readonly string[] CommandTexts = ["Attack", "Defend", "Talk", "Item", "Run", "End Turn"];

    private GridContainer _gridContainer;

    [Export] public PackedScene ChoiceContentScene { get; set; }

    public int Rows
    {
        get { return _gridContainer.GetChildCount() / _gridContainer.Columns; }
    }

    public int Columns
    {
        get { return _gridContainer.Columns; }
    }

    public int ConvertToIndex(int row, int col)
    {
        return row * Columns + col;
    }

    public ChoiceContent Get(int index)
    {
        if (index < 0 || index >= _gridContainer.GetChildCount())
        {
            GD.PrintErr($"Invalid index {index} for Commands.");
            return null;
        }
        return _gridContainer.GetChild<ChoiceContent>(index, false);
    }

    public void ShowPlayerCommands()
    {
        if (_gridContainer.GetChildCount() > 0)
        {
            Clear();
        }
        
        foreach (var text in CommandTexts)
        {
            var choiceContent = ChoiceContentScene.Instantiate<ChoiceContent>();
            _gridContainer.AddChild(choiceContent);
            choiceContent.Label.Text = text;
        }
        ShowArrow(0);
    }

    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("MarginContainer/GridContainer");
        ShowPlayerCommands();
    }

    public void Add(ChoiceContent choiceContent)
    {
        _gridContainer.AddChild(choiceContent);
    }

    public void Clear()
    {
        foreach (Node child in _gridContainer.GetChildren())
        {
            if (child is ChoiceContent choiceContent)
            {
                RemoveChild(choiceContent);
                choiceContent.QueueFree();
            }
        }
    }

    public void ShowArrow(int index)
    {
        if (index < 0 || index >= _gridContainer.GetChildCount())
        {
            GD.PrintErr($"Invalid index {index} for Commands.");
            return;
        }

        var choiceContent = _gridContainer.GetChild<ChoiceContent>(index, false);
        choiceContent.ShowArrow();
    }

    public void HideArrow(int index)
    {
        if (index < 0 || index >= _gridContainer.GetChildCount())
        {
            GD.PrintErr($"Invalid index {index} for Commands.");
            return;
        }

        var choiceContent = _gridContainer.GetChild<ChoiceContent>(index, false);
        choiceContent.HideArrow();
    }
}
