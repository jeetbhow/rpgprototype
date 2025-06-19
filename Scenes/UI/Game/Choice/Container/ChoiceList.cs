using Godot;

public partial class ChoiceList : VBoxContainer
{
    public ChoiceButton[] GetChoiceButtons()
    {
        int size = GetChildCount();
        ChoiceButton[] choiceButtons = new ChoiceButton[size];

        for (int i = 0; i < size; i++)
        {
            ChoiceButton btn = GetChild<ChoiceButton>(i, true);
            _ = choiceButtons[i] = btn;
        }
        return choiceButtons;
    }
}
