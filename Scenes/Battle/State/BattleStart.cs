using Godot;
using System.Threading.Tasks;

public partial class BattleStart : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattleNPCTurn { get; set; }
    [Export] public StateNode BattlePlayerTurn { get; set; }

    public override async Task Enter()
    {
        await Battle.Init();

        Battle.DetermineTurnOrder();
        Battle.CurrFighter = Battle.TurnQueue.Dequeue();

        await Battle.UI.Log.AppendLine(
            $"{Battle.CurrFighter.Name} is ready to fight!"
        );

        // Emit a signal to go to the state node corresponding to either the Player or NPC turn.
        switch (Battle.CurrFighter)
        {
            case Enemy:
                EmitSignal(SignalName.StateUpdate, BattleNPCTurn.Name);
                break;
            case Player:
                EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                break;
            case Ally:
                EmitSignal(SignalName.StateUpdate, BattleNPCTurn.Name);
                break;
        }
    }
}
