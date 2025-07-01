using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class StateNode : Node
{
    [Signal]
    public delegate void StateUpdateEventHandler(string eventName);
    
    public override void _Ready()
    {
        SetProcessUnhandledInput(false);
        SetProcessInput(false);
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    public virtual Task Enter() => Task.CompletedTask;
    public virtual Task Exit() => Task.CompletedTask;
}
