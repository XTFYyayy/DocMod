using Godot;
using System;

namespace Doc.DocCode.Nodes;

/// <summary>
/// 通用召唤物视觉基类，不依赖 Spine 动画
/// </summary>
public partial class NSummonVisuals : Node2D
{
    protected Sprite2D _sprite;
    protected Control _bounds;
    protected Marker2D _centerPos;
    protected Marker2D _intentPos;

    public override void _Ready()
    {
        // 创建 Sprite
        _sprite = GetNode<Sprite2D>("Sprite");
        if (_sprite == null)
        {
            _sprite = new Sprite2D();
            _sprite.Name = "Sprite";
            AddChild(_sprite);
        }

        // 创建 Bounds（交互区域）
        _bounds = GetNode<Control>("Bounds");
        if (_bounds == null)
        {
            _bounds = new Control();
            _bounds.Name = "Bounds";
            _bounds.OffsetLeft = -121;
            _bounds.OffsetTop = -280;
            _bounds.OffsetRight = 121;
            _bounds.MouseFilter = Control.MouseFilterEnum.Ignore;
            AddChild(_bounds);
        }

        // 创建 CenterPos
        _centerPos = GetNode<Marker2D>("CenterPos");
        if (_centerPos == null)
        {
            _centerPos = new Marker2D();
            _centerPos.Name = "CenterPos";
            _centerPos.Position = new Vector2(0, -165);
            AddChild(_centerPos);
        }

        // 创建 IntentPos
        _intentPos = GetNode<Marker2D>("IntentPos");
        if (_intentPos == null)
        {
            _intentPos = new Marker2D();
            _intentPos.Name = "IntentPos";
            _intentPos.Position = new Vector2(20, -351);
            AddChild(_intentPos);
        }

        OnReady();
    }

    protected virtual void OnReady()
    {
        // 子类可重写此方法进行额外初始化
    }

    /// <summary>
    /// 设置召唤物的纹理
    /// </summary>
    public virtual void SetTexture(Texture2D texture)
    {
        _sprite.Texture = texture;
    }

    /// <summary>
    /// 设置召唤物的位置偏移
    /// </summary>
    public virtual void SetOffset(Vector2 offset)
    {
        _sprite.Position = offset;
    }

    /// <summary>
    /// 设置召唤物的大小
    /// </summary>
    public virtual void SetScale(Vector2 scale)
    {
        _sprite.Scale = scale;
    }

    /// <summary>
    /// 播放受击效果
    /// </summary>
    public virtual async System.Threading.Tasks.Task PlayHitEffect()
    {
        // 简单的闪烁效果
        _sprite.Modulate = Colors.Red;
        await System.Threading.Tasks.Task.Delay(100);
        _sprite.Modulate = Colors.White;
    }

    /// <summary>
    /// 播放死亡效果
    /// </summary>
    public virtual async System.Threading.Tasks.Task PlayDeathEffect()
    {
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(_sprite, "modulate:a", 0, 0.5);
        tween.TweenProperty(_sprite, "scale", Vector2.Zero, 0.5);
        await ToSignal(tween, Tween.SignalName.Finished);
        QueueFree();
    }
}