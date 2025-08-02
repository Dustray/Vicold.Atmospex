using Vicold.Atmospex.Data.Vector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vicold.Atmospex.Data
{
    public class LineData : IVectorData
    {
        private IList<Line> _data;

        public LineData()
        {
            _data = new List<Line>();
        }

        public int Count => _data.Count;

        public void Add(float[] data, float width, float value, Color color, PolygonType polygonType = PolygonType.Line)
        {
            Add(data, width, value, color, Color.White, polygonType);
        }

        public void Add(float[] data, float width, float value,
            Color color, Color fillColor, PolygonType polygonType = PolygonType.Line | PolygonType.Fill)
        {
            _data.Add(new Line()
            {
                Data = data,
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

        public class Line
        {
            public float[] Data
            {
                get; set;
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
