using Godot;
using System.Threading.Tasks;

using Combat.Actors;
using Items;

public partial class Barter : StateNode
{
    [Export] Battle Battle { get; set; }
    [Export] StateNode PlayerTurn { get; set; }

    private Enemy _enemy;

    public override async void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Accept"))
        {
            int index = Battle.UI.Commands.Choices.SelectedIndex;
            Item item = _enemy.HeldItems[index];
            Game.Instance.Player.Money -= item.Value;
            
            Battle.UI.Commands.TextLabel.Text = $"Money: ${Game.Instance.Player.Money}";

            await Battle.UI.Log.AppendLine($"You purchased the {item.Name} from {_enemy.Name}.");

            _enemy.HeldItems.RemoveAt(index);

            Game.Instance.Player.Inventory.Add(item);

            EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
        }
    }


    public override async Task Enter()
    {
        CreateCommands();
        await Task.CompletedTask;
    }

    public void CreateCommands()
    {
        Battle.UI.Commands.TextLabel.Text = $"Money: ${Game.Instance.Player.Money}";
        Battle.UI.Commands.TextLabel.Visible = true;

        _enemy = Battle.FighterTargetedByPlayer as Enemy;
        foreach (Item item in _enemy.HeldItems)
        {
            Battle.UI.Commands.Choices.AddChoice(item.Name + $"\t${item.Value}");
        }
        Battle.UI.Commands.Choices.HideAllArrows();
        Battle.UI.Commands.Choices.ShowArrow(0);
        Battle.UI.Commands.Choices.Active = true;
    }

    public override async Task Exit()
    {
        Battle.UI.Commands.TextLabel.Visible = false;
        Battle.UI.Commands.Choices.RemoveAll();
        Battle.UI.Commands.Choices.Active = false;
    }

}
