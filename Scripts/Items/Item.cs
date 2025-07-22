using Godot;

namespace Items;

public enum ItemID
{
    None,
    BaseballBat,
    Knife,
    Katana,
    SlimeJelly
}

[GlobalClass]
public partial class Item : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public ItemID ID { get; set; }

    [Export]
    public int Value { get; set; }

    [Export]
    public string Description { get; set; }
}