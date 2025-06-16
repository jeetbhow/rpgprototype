using Godot;
using System;
using System.Threading.Tasks;

public partial class Walk : StateNode
{
    [Export]
    public Party Party { get; set; }
    private Vector2 _dir;

    public override void _PhysicsProcess(double delta)
    {
        Move();
        IdleIfNotMoving();
    }

    private void Move()
    {
        _dir = InputManager.GetInputVector();

        switch (_dir)
        {
            case var d when d == Vector2.Up:
                Party.PlayPartyLeaderAnimation("WalkUp");
                Party.Facing = NPCDataType.Facing.UP;
                break;

            case var d when d == Vector2.Right:
                Party.PlayPartyLeaderAnimation("WalkRight");
                Party.Facing = NPCDataType.Facing.RIGHT;
                break;

            case var d when d == Vector2.Down:
                Party.PlayPartyLeaderAnimation("WalkDown");
                Party.Facing = NPCDataType.Facing.DOWN;
                break;

            case var d when d == Vector2.Left:
                Party.PlayPartyLeaderAnimation("WalkLeft");
                Party.Facing = NPCDataType.Facing.LEFT;
                break;
        }
        
        Party.PartyLeader.Velocity = _dir * Party.MoveSpeed;
    }

    private void IdleIfNotMoving()
    {
        if (_dir == Vector2.Zero)
        {
            EmitSignal(SignalName.StateUpdate, "Idle");
        }
    }

    public override async Task Enter()
    {
        if (Party == null)
            throw new InvalidOperationException("Expected a valid reference to the party upon state entry.");
    }

    public override async Task Exit() {}

}
