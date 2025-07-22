using Godot;
using System.Threading.Tasks;

namespace Combat;

public partial class BattleStart : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode TurnStart { get; set; }

    public override async Task Enter()
    {
        await ToSignal(Battle, Battle.SignalName.BattleReady);
        GD.Print("Battle started");
        EmitSignal(SignalName.StateUpdate, TurnStart.Name);
    }
}
