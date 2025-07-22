using Godot;

using Combat.Actors;
using Combat.Talk;

[GlobalClass]
public partial class BrandishKnife : TalkAction
{
    public override bool IsVisible(Player player, Enemy enemy)
    {
        return enemy.WeaknessExposed;
    }
}
