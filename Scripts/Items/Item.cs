using Godot;

namespace Items;

public enum ItemID
{
    None,
    BaseballBat,
    Knife
}

[GlobalClass]
public partial class Item : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public ItemID ID { get; set; }

    [Export]
    public string Description { get; set; }
}