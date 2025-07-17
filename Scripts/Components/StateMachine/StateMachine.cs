using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export]
    public StateNode CurrStateNode { get; set; }

    private readonly Dictionary<string, StateNode> stateNodes = [];

    public override async void _Ready()
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
            stateNode.StateUpdate += async (stateNodeName) => await OnStateTransition(stateNodeName);
        }

        await EnableStateNode(CurrStateNode);
    }

    private async Task OnStateTransition(string stateNodeName)
    {

        if (!stateNodes.TryGetValue(stateNodeName, out var newStateNode))
            throw new InvalidOperationException("Attempted to transition to a state that doesn't exist.");

        await DisableStateNode(CurrStateNode);
        await EnableStateNode(newStateNode);
        CurrStateNode = newStateNode;
    }

    public static async Task EnableStateNode(StateNode stateNode)
    {
        await stateNode.Enter();
        stateNode.SetProcessUnhandledInput(true);
        stateNode.SetProcessInput(true);
        stateNode.SetProcess(true);
        stateNode.SetPhysicsProcess(true);
    }

    public static async Task DisableStateNode(StateNode stateNode)
    {
        stateNode.SetProcessUnhandledInput(false);
        stateNode.SetProcessInput(false);
        stateNode.SetProcess(false);
        stateNode.SetPhysicsProcess(false);
        await stateNode.Exit();
    }
}
