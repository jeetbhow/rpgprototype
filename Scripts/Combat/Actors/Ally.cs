using Godot;
using Items;

namespace Combat.Actors;

[GlobalClass]
public partial class Ally : Fighter
{
    [Export]
    public Weapon Weapon {get; set;}
}
