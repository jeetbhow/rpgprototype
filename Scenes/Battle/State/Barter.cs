using Godot;
using System.Threading.Tasks;

public partial class Barter : StateNode
{
    [Export] Battle Battle { get; set; }
    [Export] StateNode PlayerTurn { get; set; }

    public override async Task Enter()
    {
        GD.Print("Entered the Barter State.");
        Battle.UI.Commands.TextLabel.Text = $"Your cash: {Game.Instance.Player.Money}";
        await Task.CompletedTask;
    }

}
