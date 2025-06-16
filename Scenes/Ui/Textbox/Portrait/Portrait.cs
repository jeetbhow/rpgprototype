using Godot;

public partial class Portrait : PanelContainer
{
    [Export] TextureRect Texture { get; set; }

    private Vector2 _initSize;

    public override void _Ready()
    {
        _initSize = CustomMinimumSize;
    }

    public new void Hide()
    {
        Visible = false;
        CustomMinimumSize = Vector2.Zero;
    }

    public new void Show()
    {
        Visible = true;
        CustomMinimumSize = _initSize;
    }

    public new Vector2 GetSize()
    {
        return _initSize;
    }

    public void SetTexture(Texture2D tex)
    {
        Texture.Texture = tex;
    }
}
