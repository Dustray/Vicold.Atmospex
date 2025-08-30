using Evergine.Framework;
using Evergine.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Components;

namespace Vicold.Atmospex.Render.Frame
{
    public class NodeFactory
    {
        public static ILayerNode CreateLinesNode(string iD, Vicold.Atmospex.Data.Providers.IVectorDataProvider provider, Vicold.Atmospex.Earth.Projection.IProjection projection)
        {
            //var vectorData = provider.GetData();
            //var map = VisionCore.Current.GlobalBus.GetTransport<IMapTransport>();
            //if (vectorData is LineData lineData)
            //{
            //    var lines = LineConverter.ToVectorLines(lineData, prj);


            //    var _batchLine = new EverBatchLine(provider.);
            //    var dummyEntity = new Entity("Grid")
            //    .AddComponent(new Transform3D())
            //    .AddComponent(_batchLine);
            //    var linesNode = new LinesNode(map, lines);
            //    linesNode.ID = ID;
            //    return linesNode;
            //}
            throw new NotImplementedException();

        }

        public static ILayerNode CreateVectorsNode(string iD, IVectorDataProvider provider, IProjection projection)
        {
            throw new NotImplementedException();
        }

        //public static void ReCreateLinesNode(LinesNode layerNode, LineData lineData, IProjection projection)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
