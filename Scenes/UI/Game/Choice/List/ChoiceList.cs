using Godot;

using Combat;

public partial class ChoiceList : VBoxContainer
{
    [Export]
    public PackedScene ChoiceContentScene { get; set; }

    public bool Active { get; set; } = false;
    public int Count { get; private set; } = 0;
    private int _index = 0;


    public override void _Input(InputEvent @event)
    {
        if (!Active || @event is not InputEventKey keyEvent || !keyEvent.IsPressed())
        {
            return;
        }

        GetViewport().SetInputAsHandled();
        int prevIndex = _index;
        int numCmds = GetChoices().Length;

        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                _index = (_index + 1) % numCmds;
                SwapArrows(prevIndex, _index);
                return;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                _index = (_index - 1 + numCmds) % numCmds;
                SwapArrows(prevIndex, _index);
                return;
        }
    }
    public override void _Ready()
    {
        SignalHub.Instance.EnemySelected += OnEnemySelected;
    }

    public void OnEnemySelected(Enemy enemy, int index)
    {
        HideAllArrows();
        ShowArrow(index);
    }

    public void AddChoice(ChoiceContent btn)
    {
        AddChild(btn);
        Count++;
    }

    public void AddChoice(string text)
    {
        ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
        choice.Label.Text = text;
        AddChild(choice);
        Count++;
    }

    public void HideAllArrows()
    {
        foreach (Node child in GetChildren())
        {
            if (child is ChoiceContent choice)
            {
                choice.HideArrow();
            }
        }
    }

    public void HideArrow(int index)
    {
        ChoiceContent choice = GetChild<ChoiceContent>(index, false);
        choice.HideArrow();
    }

    public void ShowArrow(int index)
    {
        ChoiceContent choice = GetChild<ChoiceContent>(index, false);
        choice.ShowArrow();
    }

    public ChoiceContent GetChoice(int index)
    {
        ChoiceContent choice = GetChild<ChoiceContent>(index, false);
        if (choice == null)
        {
            GD.PrintErr("Choice at index " + index + " is null.");
            return null;
        }
        return choice;
    }

    public ChoiceContent[] GetChoices()
    {
        int size = GetChildCount();
        ChoiceContent[] choiceButtons = new ChoiceContent[size];

        for (int i = 0; i < size; i++)
        {
            ChoiceContent btn = GetChild<ChoiceContent>(i, false);
            _ = choiceButtons[i] = btn;
        }
        return choiceButtons;
    }

    public void RemoveAll()
    {
        foreach (Node child in GetChildren())
        {
            if (child is ChoiceContent choice)
            {
                RemoveChild(choice);
                choice.QueueFree();
            }
        }
        Count = 0;
        _index = 0;
    }
    
    public ChoiceContent GetSelectedChoice()
    {
        return GetChoice(_index);
    }

    public int GetSelectedIndex()
    {
        return _index;
    }

    private void SwapArrows(int prevIndex, int newIndex)
    {
        if (prevIndex != newIndex)
        {
            HideArrow(prevIndex);
            ShowArrow(newIndex);
        }
    }
}
