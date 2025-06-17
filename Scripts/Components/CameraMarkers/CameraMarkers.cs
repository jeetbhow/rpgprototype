using Godot;
using System;

[GlobalClass]
public partial class CameraMarkers : Node2D
{
    [Export]
    public Party Party { get; set; }

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            var marker = (Marker2D)child;
            Camera2D cam = Party.Camera;
            string name = marker.Name;
            switch (name)
            {
                case "Top":
                    cam.LimitTop = (int)marker.GlobalPosition.Y;
                    break;
                case "Bottom":
                    cam.LimitBottom = (int)marker.GlobalPosition.Y;
                    break;
                case "Right":
                    cam.LimitRight = (int)marker.GlobalPosition.X;
                    break;
                case "Left":
                    cam.LimitLeft = (int)marker.GlobalPosition.X;
                    break;
            }
        }
    }   
}