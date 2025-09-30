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

    private readonly VectorLine[] _lines;

    public LinesNode(VectorLine[] data)
    {
        _lines = data;
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


    //public override void _Draw()
    //{
    //    base._Draw();
    //    //foreach (var line in _lines)
    //    //{
    //    //    if (line.Width != 1)
    //    //    {
    //    //        continue;
    //    //    }

    //    //    //DrawPolygon(line.Data, new Color[] { line.LineColor });
    //    //    DrawPolyline(line.Data, line.LineColor, 1);
    //    //}
    //}
    public void ResetScale(float scale)
    {
        _scale = scale;
        //Update();
    }

    //private void OnMapChanged(object sender, MapChangedEventArgs e)
    //{
    //    _scale = e.Scale;
    //    var children = GetChildren();
    //    foreach (var child in children)
    //    {
    //        var node = child as GoLine2D;
    //        if (node != null)
    //        {
    //            node.Width = node.SourceWidth * e.Scale;
    //        }
    //    }
    //}

    //protected override void Dispose(bool disposing)
    //{
    //    base.Dispose(disposing);
    //    if (disposing)
    //    {
    //        _map.OnMapChanged -= OnMapChanged;
    //        _lines = null;
    //    }
    //}

    //private void ThreadAdd()
    //{
    //    foreach (var line in _lines)
    //    {
    //        //if (max < line.Data.Length) max = line.Data.Length;
    //        if ((line.PolygonType & PolygonType.Fill) != PolygonType.None)
    //        {
    //            var polygon2D = new Polygon2D();
    //            polygon2D.Polygon = line.Data;
    //            polygon2D.Color = line.FillColor;
    //            //AddChild(polygon2D);
    //            CallDeferred("add_child", polygon2D);
    //            //var dd = Geometry.MergePolygons2d(line.Data, null);
    //            //foreach (var pp in dd)
    //            //{
    //            //    var polygon2D = new Polygon2D();
    //            //    polygon2D.Polygon = (Vector2[])pp;
    //            //    polygon2D.Color = line.FillColor;
    //            //    //AddChild(polygon2D);
    //            //    CallDeferred("add_child", polygon2D);
    //            //}
    //        }

    //        //if (line.Width == 1)
    //        //{
    //        //    continue;
    //        //}

    //        if ((line.PolygonType & PolygonType.Line) != PolygonType.None)
    //        {
    //            var line2D = new GoLine2D();
    //            line2D.Points = line.Data;
    //            line2D.DefaultColor = line.LineColor;
    //            line2D.Width = line.Width * _scale;
    //            line2D.SourceWidth = line.Width;
    //            //AddChild(line2D);
    //            CallDeferred("add_child", line2D);

    //        }
    //    }
    //}

    //public void ResetLines(VectorLine[] data)
    //{
    //    _lines = data;
    //    var children = GetChildren();
    //    for (var i = children.Count - 1; i >= 0; i--)
    //    {
    //        var node = (Node)children[i];
    //        RemoveChild(node);
    //    }

    //    var t = new Thread();
    //    t.Start(this, "ThreadAdd");
    //    t.WaitToFinish();
    //    t.Dispose();
    //}

    public void Dispose()
    {
    }

    public void ResetLines(VectorLine[] lines) => throw new NotImplementedException();
}