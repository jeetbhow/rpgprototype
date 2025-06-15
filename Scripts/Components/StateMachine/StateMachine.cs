using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export]
    public StateNode CurrStateNode { get; set; }

    private readonly Dictionary<string, StateNode> stateNodes = new();

    public override void _Ready()
    {
        if (CurrStateNode == null)
            throw new InvalidOperationException("The current state node should be initialized.");

        foreach (Node child in GetChildren())
        {
            if (child is not StateNode stateNode)
            {
                throw new InvalidOperationException("Child must be an instance of StateNode.");
            }

            stateNodes[stateNode.Name.ToString()] = stateNode;
            stateNode.StateUpdate += OnStateTransition;
        }

        EnableStateNode(CurrStateNode);
    }

    private void OnStateTransition(string stateNodeName)
    {
        DisableStateNode(CurrStateNode);
        if (!stateNodes.TryGetValue(stateNodeName, out var newStateNode))
            throw new InvalidOperationException("Attempted to transition to a state that doesn't exist.");
        EnableStateNode(newStateNode);
        CurrStateNode = newStateNode;
    }

    public static void EnableStateNode(StateNode stateNode)
    {
        stateNode.Enter();
        stateNode.SetProcessInput(true);
        stateNode.SetProcess(true);
        stateNode.SetPhysicsProcess(true);
    }

    public static void DisableStateNode(StateNode stateNode)
    {
        stateNode.SetProcessInput(false);
        stateNode.SetProcess(false);
        stateNode.SetPhysicsProcess(false);
        stateNode.Exit();
    }
}
