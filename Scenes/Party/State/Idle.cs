using Godot;
using System;
using System.Threading.Tasks;

public partial class Idle : StateNode
{
    [Export]
    public Party Party;

    public override void _PhysicsProcess(double delta)
    {
        Walk();
    }

    private void Walk()
    {
        Vector2 dir = InputManager.GetInputVector();
        if (dir != Vector2.Zero)
        {
            EmitSignal(SignalName.StateUpdate, "Walk");
            return;
        }
    }

    public override async Task Enter()
    {
        switch (Party.Facing)
        {
            case NPCDataType.Facing.UP:
                Party.PlayPartyLeaderAnimation("IdleUp");
                break;
            case NPCDataType.Facing.RIGHT:
                Party.PlayPartyLeaderAnimation("IdleRight");
                break;
            case NPCDataType.Facing.DOWN:
                Party.PlayPartyLeaderAnimation("IdleDown");
                break;
            case NPCDataType.Facing.LEFT:
                Party.PlayPartyLeaderAnimation("IdleLeft");
                break;
        }
    }

    public override async Task Exit() {}
}
