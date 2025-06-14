using Godot;
using System;

[GlobalClass]
public partial class PartyMember : CharacterBody2D
{
    // --- exported property ---
    [Export]
    public NPCDataType.Facing Facing { get; set; }

    // --- runtime references ---
    public AnimatedSprite2D _animatedSprite2D;
    private CollisionShape2D _collisionShape2D;

    // --- leader flag ---
    private bool _isPartyLeader = false;
    public bool IsPartyLeader
    {
        get => _isPartyLeader;
        set => _isPartyLeader = value;
    }

    public override void _Ready()
    {
        // cache child nodes
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

        // hide/disable collisions if under a Party
        if (GetParent() is Party)
        {
            _collisionShape2D.Disabled = true;
            Visible = false;
        }
        else
        {
            Visible = true;
            _collisionShape2D.Disabled = false;
        }

        // play the correct idle animation based on facing
        switch (Facing)
        {
            case NPCDataType.Facing.UP:
                _animatedSprite2D.Play("IdleUp");
                break;
            case NPCDataType.Facing.DOWN:
                _animatedSprite2D.Play("IdleDown");
                break;
            case NPCDataType.Facing.LEFT:
                _animatedSprite2D.Play("IdleLeft");
                break;
            case NPCDataType.Facing.RIGHT:
                _animatedSprite2D.Play("IdleRight");
                break;
        }
    }

    public void EnableCollisionShape()
    {
        _collisionShape2D.Disabled = false;
    }

    // called externally to trigger any animation
    public void PlayAnimation(string animName)
    {
        _animatedSprite2D.Play(animName);
    }
}
