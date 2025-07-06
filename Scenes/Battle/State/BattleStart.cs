using Godot;
using System;
using System.Threading.Tasks;

public partial class BattleStart : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode TurnStart { get; set; }

    public override async Task Enter()
    {
        try
        {
            await Battle.Init();
            await Battle.DetermineTurnOrder();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Battle start failed: {e}");
        }

        EmitSignal(SignalName.StateUpdate, TurnStart.Name);
    }
}
