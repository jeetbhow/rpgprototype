using Godot;
using Items;

namespace Combat;

[GlobalClass]
public partial class Ally : Fighter
{
    [Export]
    public Weapon Weapon {get; set;}
}
