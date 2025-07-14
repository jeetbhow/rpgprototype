using Godot;

namespace Combat;

[GlobalClass]
public partial class AIAction : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public string Message { get; set; }

    [Export]
    public bool HasDmg { get; set; } = false;

    [Export]
    public Ability Ability { get; set; }

    public bool CanExecute(int ap)
    {
        return Ability.APCost <= ap;
    }
}
