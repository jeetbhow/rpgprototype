using Godot;

public partial class Portrait : PanelContainer
{
    [Export] public TextureRect Texture { get; set; }

    public void SetTexture(Texture2D tex)
    {
        Texture.Texture = tex;
    }
}
