using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Projection
{
    public struct ProjectionInfo
    {
        public string Name
        {
            get; internal set;
        }
        public float North
        {
            get; internal set;
        }
        public float South
        {
            get; internal set;
        }
        public float West
        {
            get; internal set;
        }
        public float East
        {
            get; internal set;
        }

        public float MapLonCenter
        {
            get; internal set;
        }
        public readonly float LonCenter => (East + West) / 2;
        public readonly float LatCenter => (North + South) / 2;

        public float WorldScale
        {
            get; set;
        }

    }
}
