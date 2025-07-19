using Godot;
using System.Threading.Tasks;

using Combat.Actors;

public partial class TurnStart : StateNode
{
    [Export]
    Battle Battle { get; set; }

    [Export]
    StateNode NPCTurn { get; set; }

    [Export]
    StateNode PlayerTurn { get; set; }

    public override async Task Enter()
    {
        Battle.CurrFighter = Battle.TurnQueue.Peek();
        switch (Battle.CurrFighter)
        {
            case Enemy:
                EmitSignal(SignalName.StateUpdate, NPCTurn.Name);
                break;
            case Player:
                EmitSignal(SignalName.StateUpdate, PlayerTurn.Name);
                break;
            case Ally:
                EmitSignal(SignalName.StateUpdate, NPCTurn.Name);
                break;
        }
    }
}
