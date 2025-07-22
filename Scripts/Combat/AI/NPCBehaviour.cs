using Godot;
using Combat.Actors;
using Combat.Attack;

namespace Combat.AI;

[GlobalClass]
public abstract partial class NPCBehaviour : Resource
{
    public abstract (Fighter target, NPCAction action) DecideTurn(
        NPCFighter self,
        Enemy[] enemies,
        Fighter[] party
    );
}
