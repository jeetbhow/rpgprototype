using Godot;
using System;
using Godot.Collections;

[GlobalClass]
public partial class Party : Node2D
{
    [Export] public NPCDataType.Facing Facing { get; set; }
    [Export] public Array<PartyMember> PartyMembers { get; set; }
    [Export] public PartyMember PartyLeader { get; set; }
    [Export] public Camera2D Camera { get; set; }
    [Export] public float MoveSpeed { get; set; } = 30.0f;
    [Export] public float PartyFollowDistance { get; set; } = 15.0f;
    [Export] public float PartyFollowLag { get; set; } = 15.0f;
    
    private EventBus eventBus;

    public override void _Ready()
    {
        eventBus = GetNode<EventBus>(EventBus.Path);
        eventBus.PartyMemberAdded += OnPartyMemberAdded;

        RemoveChild(Camera);
        PartyLeader.AddChild(Camera);
        UpdateParty();
    }

    public void UpdateParty()
    {
        foreach (PartyMember member in PartyMembers)
        {
            if (member == PartyLeader)
            {
                member.IsPartyLeader = true;
                InitCollisions(member, CollisionObject.CollisionLayer.PARTY_LEADER);
            }
            else
            {
                member.IsPartyLeader = false;
                InitCollisions(member, CollisionObject.CollisionLayer.PARTY_MEMBER);
            }
            member.Visible = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        PartyLeader.MoveAndSlide();
        foreach (PartyMember member in PartyMembers)
        {
            if (member == PartyLeader)
            {
                continue;
            }
            FollowLeader(member);
            member.MoveAndSlide();
        }
    }

    private void FollowLeader(PartyMember member)
    {
        var offset = Facing switch
        {
            NPCDataType.Facing.UP => new Vector2(0, PartyFollowDistance),
            NPCDataType.Facing.DOWN => new Vector2(0, -PartyFollowDistance),
            NPCDataType.Facing.LEFT => new Vector2(PartyFollowDistance, 0),
            NPCDataType.Facing.RIGHT => new Vector2(-PartyFollowDistance, 0),
            _ => Vector2.Zero,
        };

        var target = PartyLeader.GlobalPosition + offset;
        var dir = target - member.GlobalPosition;

        if (dir.Length() < 5.0f)
        {
            switch (Facing)
            {
                case NPCDataType.Facing.UP: member.PlayAnimation("IdleUp"); break;
                case NPCDataType.Facing.DOWN: member.PlayAnimation("IdleDown"); break;
                case NPCDataType.Facing.LEFT: member.PlayAnimation("IdleLeft"); break;
                case NPCDataType.Facing.RIGHT: member.PlayAnimation("IdleRight"); break;
            }
            member.Velocity = Vector2.Zero;
        }
        else if (Math.Abs(dir.X) > Math.Abs(dir.Y))
        {
            member.PlayAnimation(dir.X > 0 ? "WalkRight" : "WalkLeft");
            member.Velocity = dir * (MoveSpeed / PartyFollowLag);
        }
        else
        {
            member.PlayAnimation(dir.Y > 0 ? "WalkDown" : "WalkUp");
            member.Velocity = dir * (MoveSpeed / PartyFollowLag);
        }
    }

    public void ChangeAnimSpeed(float animSpeed)
    {
        PartyLeader.AnimatedSprite2D.SpeedScale = animSpeed;
    }

    public void PlayAnimation(string animName)
    {
        foreach (PartyMember member in PartyMembers)
        {
            member.PlayAnimation(animName);
        }
    }

    public void PlayPartyLeaderAnimation(string animName)
    {
        PartyLeader.PlayAnimation(animName);
    }

    private static void InitCollisions(PartyMember member, CollisionObject.CollisionLayer type)
    {
        switch (type)
        {
            case CollisionObject.CollisionLayer.PARTY_LEADER:
                member.SetCollisionLayerValue((int)CollisionObject.CollisionLayer.PARTY_LEADER, true);
                member.SetCollisionLayerValue((int)CollisionObject.CollisionLayer.PARTY_MEMBER, false);
                member.SetCollisionLayerValue((int)CollisionObject.CollisionLayer.OBJECT, false);
                member.SetCollisionMaskValue((int)CollisionObject.CollisionLayer.OBJECT, true);
                break;

            case CollisionObject.CollisionLayer.PARTY_MEMBER:
                member.SetCollisionLayerValue((int)CollisionObject.CollisionLayer.PARTY_MEMBER, true);
                member.SetCollisionLayerValue((int)CollisionObject.CollisionLayer.PARTY_LEADER, false);
                member.SetCollisionLayerValue((int)CollisionObject.CollisionLayer.OBJECT, false);
                member.SetCollisionMaskValue((int)CollisionObject.CollisionLayer.OBJECT, true);
                member.SetCollisionMaskValue((int)CollisionObject.CollisionLayer.PARTY_LEADER, false);
                break;
        }
        member.EnableCollisionShape();
    }

    private void OnPartyMemberAdded(PartyMember newMember)
    {
        PartyMembers.Add(newMember);
        UpdateParty();
    }
}
