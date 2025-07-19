using Godot;
using System.Threading.Tasks;

using Combat.Actors;

public partial class WeaponMenu : StateNode
{
    [Export]
    public Battle Battle { get; set; }

    [Export]
    public StateNode ItemMenu { get; set; }

    [Export]
    public StateNode PlayerTurn { get; set; }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Cancel"))
        {
            EmitSignal(SignalName.StateUpdate, ItemMenu.Name);
        }

        if (@event.IsActionPressed("Accept"))
        {
            SelectWeapon();
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
        foreach (var weapon in weapons)
        {
            if (player.Weapon == weapon)
            {
                Battle.UI.Commands.Choices.AddChoice($"{weapon.Name} (E)");
            }
            else
            {
                Battle.UI.Commands.Choices.AddChoice(weapon.Name);
            }
        }

        Battle.UI.Commands.Choices.HideAllArrows();
        Battle.UI.Commands.Choices.ShowArrow(0);
        Battle.UI.Commands.Choices.Active = true;
    }
    public void SelectWeapon()
    {
        int index = Battle.UI.Commands.Choices.SelectedIndex;

        var player = Battle.CurrFighter as Player;
        player.Weapon = player.GetWeapons()[index];
        player.AP -= Game.WeaponSwapAPCost;
        
        EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
    }
}
