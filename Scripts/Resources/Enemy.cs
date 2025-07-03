using Godot;

[GlobalClass]
public partial class Enemy : Fighter
{
    [Export] public string Introduction { get; set; }
    [Export] public FighterAI AI { get; set; }
}
