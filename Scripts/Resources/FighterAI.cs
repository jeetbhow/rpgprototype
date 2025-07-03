using Godot;

[GlobalClass]
public partial class FighterAI : Resource
{
    [Export] public AIAction[] Actions { get; set; }
}
