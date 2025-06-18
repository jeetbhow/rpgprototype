using Godot;

public partial class Portrait : PanelContainer
{
    [Export] TextureRect Texture { get; set; }

    public new void Hide()
    {
        Visible = false;
    }

    public new void Show()
    {
        Visible = true;
    }

    public void SetTexture(Texture2D tex)
    {
        Texture.Texture = tex;
    }
}
