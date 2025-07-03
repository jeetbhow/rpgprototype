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

    public virtual async Task Enter() => await Task.CompletedTask;
    public virtual async Task Exit() => await Task.CompletedTask;
}
