using Godot;
using System;

namespace Vicold.Atmospex.Godot.Frame;

/// <summary>
/// 视口信息类
/// </summary>
public class ViewportInfo
{
    /// <summary>
    /// 视口屏幕坐标范围
    /// </summary>
    public Rect2I ScreenViewport { get; set; }

    /// <summary>
    /// 视口世界坐标范围
    /// </summary>
    public Rect2 WorldViewport { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 屏幕最小X坐标
    /// </summary>
    public int ScreenMinX => ScreenViewport.Position.X;

    /// <summary>
    /// 屏幕最小Y坐标
    /// </summary>
    public int ScreenMinY => ScreenViewport.Position.Y;

    /// <summary>
    /// 屏幕最大X坐标
    /// </summary>
    public int ScreenMaxX => ScreenViewport.Position.X + ScreenViewport.Size.X;

    /// <summary>
    /// 屏幕最大Y坐标
    /// </summary>
    public int ScreenMaxY => ScreenViewport.Position.Y + ScreenViewport.Size.Y;

    /// <summary>
    /// 世界最小X坐标
    /// </summary>
    public float WorldMinX => WorldViewport.Position.X;

    /// <summary>
    /// 世界最小Y坐标
    /// </summary>
    public float WorldMinY => WorldViewport.Position.Y;

    /// <summary>
    /// 世界最大X坐标
    /// </summary>
    public float WorldMaxX => WorldViewport.Position.X + WorldViewport.Size.X;

    /// <summary>
    /// 世界最大Y坐标
    /// </summary>
    public float WorldMaxY => WorldViewport.Position.Y + WorldViewport.Size.Y;

    /// <summary>
    /// 滚动缩放比例
    /// </summary>
    public float ScrollScale { get; set; } = 1f;

    public ViewportInfo()
    {
        ScreenViewport = new Rect2I();
        WorldViewport = new Rect2();
        UpdateTime = DateTime.Now;
    }

    public ViewportInfo(Rect2I screenViewport, Rect2 worldViewport)
    {
        ScreenViewport = screenViewport;
        WorldViewport = worldViewport;
        UpdateTime = DateTime.Now;
    }
}