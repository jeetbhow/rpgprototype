using Godot;
using System.Threading.Tasks;

public partial class BattleNpcTurn : StateNode
{
    [Export] public Battle Battle { get; set; }

    public override async Task Enter()
    {
        
    }
}
