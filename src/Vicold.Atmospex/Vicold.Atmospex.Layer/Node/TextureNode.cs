using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Vicold.Atmospex.Layer.Node;
public abstract  class TextureNode : ILayerNode
{
    public string ID
    {
        get; set;
    } = string.Empty;

    public float StartX
    {
        get; set;
    }

    public float StartY
    {
        get; set;
    }

    public float WorldWidth
    {
        get; set;
    }

    public float WorldHeight
    {
        get; set;
    }

    public abstract bool Visible
    {
        get;set;
    }

    public abstract void SetLevel(int zIndex);



    public void ResetScale(float scale)
    {
    }

    public abstract void Dispose();
        //GC.SuppressFinalize(this);
}
