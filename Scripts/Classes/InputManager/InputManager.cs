using Godot;
using System;

public static partial class InputManager
{
    public static Vector2 GetInputVector()
    {
        Vector2 dir = Vector2.Zero;

        if (Input.IsActionPressed("MoveUp"))
        {
            dir = Vector2.Up;
        }
        else if (Input.IsActionPressed("MoveRight"))
        {
            dir = Vector2.Right;
        }
        else if (Input.IsActionPressed("MoveDown"))
        {
            dir = Vector2.Down;
        }
        else if (Input.IsActionPressed("MoveLeft"))
        {
            dir = Vector2.Left;
        }

        return dir;
    }
}
