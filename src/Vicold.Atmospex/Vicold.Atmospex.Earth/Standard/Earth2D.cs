using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Standard
{
    public class Earth2D
    {
        private World2D _min;
        private World2D _max;

        public Earth2D()
        {
            _min = new World2D(1, 1);
            _max = World2D.Zero;
        }

        public Earth2D(double minx, double miny, double maxx, double maxy)
        {
            _min = new World2D(minx, miny);
            _max = new World2D(maxx, maxy);
        }

        public Earth2D(World2D min, World2D max)
        {
            _min = min;
            _max = max;
        }

        public World2D Minimum => _min;

        public World2D Maximum => _max;

        public double Width
        {
            get { return (_max.X - _min.X); }
        }

        public double Height
        {
            get { return (_max.Y - _min.Y); }
        }

        public bool IsEmpty
        {
            get { return (_min.X > _max.X); }
        }

        public void SetExtents(World2D min, World2D max)
        {
            _min = min;
            _max = max;
        }

        public void SetEmpty()
        {
            _min = new World2D(1, 1);
            _max = World2D.Zero;
        }

        public bool Intersects(double x, double y)
        {
            if (this.IsEmpty) return false;

            return (x >= _min.X && y <= _max.X &&
                y >= _min.Y && y <= _max.Y);
        }

        public bool Intersects(double x, double y, double width, double height)
        {
            if (this.IsEmpty) return false;

            if (this._max.X < x)
                return false;
            if (this._max.Y < y)
                return false;

            if (this._min.X > (x + width))
                return false;
            if (this._min.Y > (y + height))
                return false;

            return true;
        }

        public bool Contains(double x, double y)
        {
            if (this.IsEmpty)
                return false;

            return _min.X <= x && x <= _max.X &&
                   _min.Y <= y && y <= _max.Y;
        }

        public void Merge(Earth2D box)
        {
            if (this.IsEmpty)
            {
                _min = box._min;
                _max = box._max;
            }
            else
            {
                _min.Floor(box.Minimum);
                _max.Ceil(box.Maximum);
            }
        }

        public void Merge(World2D point)
        {
            if (this.IsEmpty)
            {
                _min = point;
                _max = point;
            }
            else
            {
                if (point.X > _max.X)
                    _max.X = point.X;
                else if (point.X < _min.X)
                    _min.X = point.X;

                if (point.Y > _max.Y)
                    _max.Y = point.Y;
                else if (point.Y < _min.Y)
                    _min.Y = point.Y;
            }
        }
    }
}