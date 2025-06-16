using Godot;
using System.Threading.Tasks;

[GlobalClass]
public abstract partial class StateNode : Node
{
    [Signal]
    public delegate void StateUpdateEventHandler(string eventName);

    public override void _Ready()
    {
        SetProcessInput(false);
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    public abstract Task Enter();
    public abstract Task Exit();
}
