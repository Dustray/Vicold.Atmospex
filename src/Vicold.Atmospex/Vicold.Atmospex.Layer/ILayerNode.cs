using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Layer;
public interface ILayerNode : IDisposable
{
    /// <summary>
    /// 图层ID
    /// </summary>
    string ID { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// 设置图层高度优先级
    /// </summary>
    /// <param name="layerLevel"></param>
    /// <param name="zIndex"></param>
    void SetLevel(int zIndex);

    /// <summary>
    /// 重设缩放
    /// </summary>
    /// <param name="scale"></param>
    void ResetScale(float scale);
}