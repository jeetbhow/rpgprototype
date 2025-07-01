using Godot;

public partial class ChoiceList : VBoxContainer
{
    [Export] public PackedScene ChoiceContentScene { get; set; }

    public void AddChoice(ChoiceContent btn)
    {
        AddChild(btn);
    }

    public void AddChoice(string text)
    {
        ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
        AddChild(choice);
        choice.Label.Text = text;
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

    public void ShowArrow(int index)
    {
        ChoiceContent choice = GetChild<ChoiceContent>(index, true);
        if (choice == null)
        {
            GD.PrintErr("Choice at index " + index + " is null.");
            return;
        }
        choice.ShowArrow();
    }

    public ChoiceContent[] GetChoices()
    {
        int size = GetChildCount();
        ChoiceContent[] choiceButtons = new ChoiceContent[size];

        for (int i = 0; i < size; i++)
        {
            ChoiceContent btn = GetChild<ChoiceContent>(i, true);
            _ = choiceButtons[i] = btn;
        }
        return choiceButtons;
    }

    public void Clear()
    {
        foreach (Node child in GetChildren())
        {
            if (child is ChoiceContent choice)
            {
                choice.QueueFree();
            }
        }
    }
}
