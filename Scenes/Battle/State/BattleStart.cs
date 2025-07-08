using Godot;
using System.Threading.Tasks;

public partial class BattleStart : StateNode
{
    [Export]
    public Battle Battle { get; set; }

    [Export]
    public StateNode TurnStart { get; set; }

    public override async Task Enter()
    {
        await ToSignal(Battle, Battle.SignalName.BattleReady);
        EmitSignal(SignalName.StateUpdate, TurnStart.Name);
    }
}
