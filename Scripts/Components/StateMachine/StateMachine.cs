using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export]
    public StateNode CurrStateNode { get; set; }

    private readonly Dictionary<string, StateNode> _stateNodes = new();

    public override void _Ready()
    {
        if (CurrStateNode == null)
            throw new InvalidOperationException("The current state node should be initialized.");

        // Register all child StateNode instances
        foreach (Node child in GetChildren())
        {
            if (child is not StateNode stateNode)
            {
                throw new InvalidOperationException("Child must be an instance of StateNode.");
            }

            _stateNodes[stateNode.Name.ToString()] = stateNode;
            // Connect the GDScript signal "state_update" to our C# handler
            stateNode.StateUpdate += OnStateTransition;
        }

        // Activate the initial state
        CurrStateNode.Enter();
        CurrStateNode.SetProcessInput(true);
        CurrStateNode.SetProcess(true);
        CurrStateNode.SetPhysicsProcess(true);
    }

    private void OnStateTransition(string stateNodeName)
    {
        // Deactivate current
        CurrStateNode.SetProcessInput(false);
        CurrStateNode.SetProcess(false);
        CurrStateNode.SetPhysicsProcess(false);
        CurrStateNode.Exit();

        // Look up the new state
        if (!_stateNodes.TryGetValue(stateNodeName, out var newStateNode))
            throw new InvalidOperationException("Attempted to transition to a state that doesn't exist.");

        // Activate new
        newStateNode.Enter();
        newStateNode.SetProcessInput(true);
        newStateNode.SetProcess(true);
        newStateNode.SetPhysicsProcess(true);

        CurrStateNode = newStateNode;
    }
}
