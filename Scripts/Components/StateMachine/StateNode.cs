using Godot;
using System;

[GlobalClass]
public partial class StateNode : Node
{
    [Signal]
    public delegate void StateUpdateEventHandler(string eventName);

    public override void _Ready()
    {
        // Disable processing until explicitly enabled
        SetProcessInput(false);
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    /// <summary>
    /// Called when entering this state.
    /// </summary>
    public virtual void Enter()
    {
        // Enter state logic
    }

    /// <summary>
    /// Called when exiting this state.
    /// </summary>
    public virtual void Exit()
    {
        // Exit state logic
    }
}
