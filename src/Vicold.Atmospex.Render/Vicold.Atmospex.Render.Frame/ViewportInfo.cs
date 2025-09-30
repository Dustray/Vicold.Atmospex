using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Render.Frame;

/// <summary>
/// 视口信息类
/// </summary>
public class ViewportInfo
{
    /// <summary>
    /// 视口屏幕坐标范围
    /// </summary>
    public Rectangle ScreenViewport { get; set; }

    /// <summary>
    /// 视口世界坐标范围
    /// </summary>
    public RectangleF WorldViewport { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 屏幕最小X坐标
    /// </summary>
    public int ScreenMinX => (int)ScreenViewport.X;

    /// <summary>
    /// 屏幕最小Y坐标
    /// </summary>
    public int ScreenMinY => (int)ScreenViewport.Y;

    /// <summary>
    /// 屏幕最大X坐标
    /// </summary>
    public int ScreenMaxX => (int)(ScreenViewport.X + ScreenViewport.Width);

    /// <summary>
    /// 屏幕最大Y坐标
    /// </summary>
    public int ScreenMaxY => (int)(ScreenViewport.Y + ScreenViewport.Height);

    /// <summary>
    /// 世界最小X坐标
    /// </summary>
    public float WorldMinX => WorldViewport.X;

    /// <summary>
    /// 世界最小Y坐标
    /// </summary>
    public float WorldMinY => WorldViewport.Y;

    /// <summary>
    /// 世界最大X坐标
    /// </summary>
    public float WorldMaxX => WorldViewport.X + WorldViewport.Width;

    /// <summary>
    /// 世界最大Y坐标
    /// </summary>
    public float WorldMaxY => WorldViewport.Y + WorldViewport.Height;

    public ViewportInfo()
    {
        ScreenViewport = Rectangle.Empty;
        WorldViewport = RectangleF.Empty;
        UpdateTime = DateTime.Now;
    }

    public ViewportInfo(Rectangle screenViewport, RectangleF worldViewport)
    {
        ScreenViewport = screenViewport;
        WorldViewport = worldViewport;
        UpdateTime = DateTime.Now;
    }
}
