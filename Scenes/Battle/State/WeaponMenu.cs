using Godot;
using System.Threading.Tasks;

using Combat.Actors;
using Items;

public partial class WeaponMenu : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode ItemMenu { get; set; }
    [Export] public StateNode PlayerTurn { get; set; }

    public override async void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Cancel"))
        {
            EmitSignal(SignalName.StateUpdate, ItemMenu.Name);
        }

        if (@event.IsActionPressed("Accept"))
        {
            await SelectWeapon();
        }
    }

    public override async Task Enter()
    {
        Battle.UI.Commands.TextLabel.Text = $"Weapon swap: [color={Game.APColor.ToHtml()}]({Game.WeaponSwapAPCost} AP)[/color]";
        Battle.UI.Commands.TextLabel.Visible = true;
        ListWeapons();
        await Task.CompletedTask;
    }

    public override async Task Exit()
    {
        Battle.UI.Commands.TextLabel.Text = null;
        Battle.UI.Commands.TextLabel.Visible = false;
        Battle.UI.Commands.Choices.RemoveAll();
        Battle.UI.Commands.Choices.Active = false;
        await Task.CompletedTask;
    }

    private void ListWeapons()
    {
        var player = Battle.CurrFighter as Player;
        var weapons = player.GetWeapons();

        Battle.UI.Commands.Choices.RemoveAll();
        for (int i = 0; i < weapons.Length; i++)
        {
            Weapon currWeapon = weapons[i];
            if (player.Weapon == currWeapon)
            {
                Battle.UI.Commands.Choices.AddChoice($"{currWeapon.Name} (E)");
            }
            else
            {
                Battle.UI.Commands.Choices.AddChoice(currWeapon.Name);
            }

            if (player.AP < Game.WeaponSwapAPCost)
            {
                Battle.UI.Commands.Choices.ToggleChoiceAvailability(i, false);
            }
        }

        Battle.UI.Commands.Choices.HideAllArrows();
        Battle.UI.Commands.Choices.ShowArrow(0);
        Battle.UI.Commands.Choices.Active = true;
    }
    public async Task SelectWeapon()
    {
        int index = Battle.UI.Commands.Choices.SelectedIndex;

        var player = Battle.CurrFighter as Player;
        player.Weapon = player.GetWeapons()[index];
        player.AP -= Game.WeaponSwapAPCost;

        await Battle.UI.Log.AppendLine($"You swapped to the {player.Weapon.Name}.");
        EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
    }
}
