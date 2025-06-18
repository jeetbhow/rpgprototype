using Godot;

public static class InputManager
{
    public static Vector2 GetInputVector()
    {
        Vector2 dir = Vector2.Zero;

        if (Input.IsActionPressed("MoveUp"))
        {
            dir += Vector2.Up;
        }

        if (Input.IsActionPressed("MoveRight"))
        {
            dir += Vector2.Right;
        }

        if (Input.IsActionPressed("MoveDown"))
        {
            dir += Vector2.Down;
        }

        if (Input.IsActionPressed("MoveLeft"))
        {
            dir += Vector2.Left;
        }

        return dir.Normalized();
    }
}
