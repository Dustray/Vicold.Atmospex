using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMIAS.VISION.Model
{
    public struct Vector4
    {
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }

        public override string ToString()
        {
            return $"left:{Left} top:{Top} right:{Right} bottom:{Bottom}";
        }
    }
}
