using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Events;

namespace Vicold.Atmospex.Layer.Node;
public abstract class LinesNode : ILayerNode
{
    private float _scale = 1;

    protected readonly List<VectorLine[]> LevelLines;

    public LinesNode(List<VectorLine[]> data)
    {
        LevelLines = data;
    }

    public string ID
    {
        get; set;
    } = string.Empty;

    public abstract bool Visible
    {
        get; set;
    }

    public abstract void SetLevel(int zIndex);

    public void ResetScale(float scale)
    {
        _scale = scale;
    }

    public abstract void Dispose();

    public void ResetLines(List<VectorLine[]> lines) => throw new NotImplementedException();
}