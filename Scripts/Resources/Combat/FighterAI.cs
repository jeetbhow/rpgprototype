using Godot;
using System.Collections.Generic;

namespace Combat;

[GlobalClass]
public partial class FighterAI : Resource
{
    [Export] public AIAction[] Actions { get; set; }
}
