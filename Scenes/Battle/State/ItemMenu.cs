using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

enum MenuType
{
    Invalid,
    Weapons,
    Consumables
}

public partial class ItemMenu : StateNode
{
    [Export]
    public Battle Battle { get; set; }

    [Export]
    public StateNode PlayerTurn { get; set; }

    [Export]
    public StateNode WeaponMenu { get; set; }

    private static readonly string[] MainMenuChoicesText =
    [
        "Weapons",
        "Consumables",
    ];

    private static readonly Dictionary<string, MenuType> MainMenuChoices = new()
    {
        { "Weapons", MenuType.Weapons },
        { "Consumables", MenuType.Consumables }
    };

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Cancel"))
        {
            EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
        }

        if (@event.IsActionPressed("Accept"))
        {
            string choice = Battle.UI.Commands.Choices.GetSelectedChoice().Label.Text;

            MenuType menuType = MainMenuChoices.GetValueOrDefault(choice, MenuType.Invalid);
            switch (menuType)
            {
                case MenuType.Weapons:
                    EmitSignal(SignalName.StateUpdate, WeaponMenu.Name);
                    break;
                case MenuType.Consumables:
                    // Handle consumable selection logic here if needed
                    break;
                default:
                    throw new InvalidOperationException($"{Name}: Invalid state menu type: {menuType}");
            }
        }
    }

    public override async Task Enter()
    {
        foreach (var menuItem in MainMenuChoicesText)
        {
            Battle.UI.Commands.Choices.AddChoice(menuItem);
        }

        Battle.UI.Commands.Choices.HideAllArrows();
        Battle.UI.Commands.Choices.ShowArrow(0);
        Battle.UI.Commands.Choices.Active = true;

        await Task.CompletedTask;
    }

    public override async Task Exit()
    {
        Battle.UI.Commands.Choices.RemoveAll();
        Battle.UI.Commands.Choices.Active = false;
        await Task.CompletedTask;
    }
}
