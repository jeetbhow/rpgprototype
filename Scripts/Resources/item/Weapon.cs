using Godot;
using Combat;

namespace Item;

[GlobalClass]
public partial class Weapon : Resource
{
    [Export]
    public string Name { get; set; }
    [Export]
    public Ability Ability { get; set; }
}
