using Godot;

using Signal;
using Combat.Actors;
using System;

public partial class ChoiceList : VBoxContainer
{
    [Export] public PackedScene ChoiceContentScene { get; set; }

    public bool Active { get; set; } = false;
    public int Count { get; private set; } = 0;
    public int SelectedIndex { get; private set; } = 0;

    public override void _Input(InputEvent @event)
    {
        if (!Active || @event is not InputEventKey keyEvent || !keyEvent.IsPressed())
        {
            return;
        }

        GetViewport().SetInputAsHandled();
        int prevIndex = SelectedIndex;
        int numCmds = GetChoices().Length;

        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                MoveSelection(1);
                return;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                MoveSelection(-1);
                return;
        }
    }

    public void MoveSelection(int delta)
    {
        if (delta == 0)
        {
            throw new ArgumentException($"ChoiceList: Invalid delta {delta}");
        }

        int prevIndex = SelectedIndex;
        do
        {
            SelectedIndex = (SelectedIndex + delta + Count) % Count;
        } while (!GetChoice(SelectedIndex).Visible);

        SwapArrows(prevIndex, SelectedIndex);

        if (SelectedIndex == 0 || SelectedIndex == Count - 1)
        {
            FocusChoice(SelectedIndex);
            return;
        }

        int focusIndex = delta > 0
            ? Math.Min(SelectedIndex + 2, Count - 1)
            : Math.Max(SelectedIndex - 2, 0);

        FocusChoice(focusIndex);
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

    public void ToggleChoiceAvailability(int index, bool enabled)
    {
        ChoiceContent choiceContent = GetChoice(index);
        choiceContent.Enabled = enabled;
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

    public void FocusChoice(int index)
    {
        ChoiceContent choice = GetChild<ChoiceContent>(index, false);
        choice.GrabFocus();
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
        SelectedIndex = 0;
    }

    public ChoiceContent GetSelectedChoice()
    {
        return GetChoice(SelectedIndex);
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
