using Godot;

using Item;

namespace Combat;

[GlobalClass]
public partial class Ally : Fighter
{
    [Export]
    public Weapon Weapon {get; set;}
}
