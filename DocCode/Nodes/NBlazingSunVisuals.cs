using Godot;

namespace Doc.DocCode.Nodes;

public partial class NBlazingSunVisuals : NSummonVisuals
{
    protected override void OnReady()
    {
        // 加载耀阳的图片
        var texture = GD.Load<Texture2D>("res://Doc/Images/Monsters/BlazingSun.png");
        SetTexture(texture);
        SetOffset(new Vector2(0, -22));
        SetScale(new Vector2(1, 1));
    }
}