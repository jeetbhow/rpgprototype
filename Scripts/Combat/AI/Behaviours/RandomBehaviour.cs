using Godot;
using Combat.Actors;

namespace Combat.AI;

[GlobalClass]
public partial class RandomBehaviour : NPCBehaviour
{
    public override (Fighter target, NPCAction action) DecideTurn(NPCFighter self, Enemy[] enemies, Fighter[] party)
    {
        NPCAction action = Game.Instance.PickRandomElement([.. self.NPCActions]);
        Fighter target = null;

        if (self is Enemy)
        {
            target = Game.Instance.PickRandomElement(party);
        }

        if (self is PartyMember)
        {
            target = Game.Instance.PickRandomElement(enemies);
        }

        return (target, action);
    }
}


