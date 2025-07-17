using Godot;

namespace Items;

[GlobalClass]
public partial class Item : Resource
{
    [Export]
    public string Name { get; set; }
}