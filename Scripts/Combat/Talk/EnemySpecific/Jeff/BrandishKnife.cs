using Godot;

using Combat.Actors;
using Combat.Talk;
using Items;

[GlobalClass]
public partial class BrandishKnife : TalkAction
{
    public override bool IsVisible(Player player, Enemy enemy)
    {
        return enemy.WeaknessExposed;
    }

    public override bool IsEnabled(Player player, Enemy enemy)
    {
        return player.Weapon.ID == ItemID.Knife;
    }

}
