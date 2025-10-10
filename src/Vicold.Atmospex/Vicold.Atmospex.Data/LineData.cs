using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Newtonsoft.Json.Linq;
using Vicold.Atmospex.Data.Vector;

namespace Vicold.Atmospex.Data
{
    public class LineData : IVectorData
    {
        private readonly IList<Line> _data = [];


        public int Count => _data.Count;

        public void Add(float[] data, float width, float value, Color color, PolygonType polygonType = PolygonType.Line)
        {
            if (polygonType == PolygonType.Line)
            {
                Add(data, width, value, color, Color.White, polygonType);
            }
            else if (polygonType == PolygonType.Fill)
            {
                Add(data, width, value, Color.White, color, polygonType);
            }
        }

        public void Add(float[] data, float width, float value, Color color, Color fillColor, PolygonType polygonType = PolygonType.Fill)
        {
            _data.Add(new Line(data)
            {
                DataSpan = data.AsMemory(),
                Color = color,
                FillColor = fillColor,
                Value = value,
                Width = width,
                PolygonType = polygonType,
            });
        }


        public void Add(float[] source, ReadOnlyMemory<float> data, float width, float value, Color color, PolygonType polygonType = PolygonType.Line)
        {
            Add(source, data, width, value, color, Color.White, polygonType);
        }
        public void Add(float[] source, ReadOnlyMemory<float> data, float width, float value, Color color, Color fillColor, PolygonType polygonType = PolygonType.Fill)
        {
            _data.Add(new Line(source)
            {
                DataSpan = data,
                Color = color,
                FillColor = fillColor,
                Value = value,
                Width = width,
                PolygonType = polygonType,
            });
        }

        public Line this[int index]
        {
            get
            {
                return _data[index];
            }
        }


        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _data.Clear();
        }


        public class Line(float[] source)
        {
            public float[] Data
            {
                get; set;
            } = source;

            public ReadOnlyMemory<float> DataSpan
            {
                get;
                internal set;
            }

            public Color Color
            {
                get; set;
            }

            public Color FillColor
            {
                get; set;
            }

            public PolygonType PolygonType
            {
                get; set;
            }

            public float Value
            {
                get; set;
            }

            public float Width
            {
                get; set;
            }

        }
    }
}
