using Godot;
using System.Threading.Tasks;

public partial class BattleStandby : StateNode
{
    [Export] public Battle Battle { get; set; }

    public override async Task Enter()
    {
        GD.Print("Entering Battle Standby State");
    }
}
