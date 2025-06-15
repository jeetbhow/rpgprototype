using Godot;

[GlobalClass]
public partial class PartyMember : CharacterBody2D
{
    [Export] public NPCDataType.Facing Facing { get; set; }
    [Export] public AnimatedSprite2D AnimatedSprite2D { get; set; }
    [Export] public CollisionShape2D CollisionShape2D { get; set; }
    [Export] public bool IsPartyLeader { get; set; }

    public override void _Ready()
    {
        if (GetParent() is Party)
        {
            CollisionShape2D.Disabled = true;
            Visible = false;
        }
        else
        {
            Visible = true;
            CollisionShape2D.Disabled = false;
        }

        // play the correct idle animation based on facing
        switch (Facing)
        {
            case NPCDataType.Facing.UP:
                AnimatedSprite2D.Play("IdleUp");
                break;
            case NPCDataType.Facing.DOWN:
                AnimatedSprite2D.Play("IdleDown");
                break;
            case NPCDataType.Facing.LEFT:
                AnimatedSprite2D.Play("IdleLeft");
                break;
            case NPCDataType.Facing.RIGHT:
                AnimatedSprite2D.Play("IdleRight");
                break;
        }
    }

    public void EnableCollisionShape()
    {
        CollisionShape2D.Disabled = false;
    }

    public void PlayAnimation(string name)
    {
        AnimatedSprite2D.Play(name);
    }
}
