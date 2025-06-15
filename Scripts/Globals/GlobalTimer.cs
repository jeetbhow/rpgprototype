using Godot;

public partial class GlobalTimer : Node
{
    public async void Wait(float time)
    {
        await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);
    }
}
