using Godot;

using Signal;
using Items;
using Combat.Actors;

namespace Combat.Talk;

[GlobalClass]
public partial class BrandishKnifeResult : TalkActionResult
{
    public override bool PrematureFailure(Player player, Enemy enemy)
    {
        if (player.Weapon.ID != ItemID.Knife)
        {
            string failMessage = "Except you aren't holding a knife, so you just awkwardly flail your arms around.";
            SignalHub.Instance.EmitSignal(SignalHub.SignalName.CombatLogUpdateRequested, failMessage);
            return true;
        }
        return false;
    }

}
