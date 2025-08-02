using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data.Vector
{
    public static class VectorCollectionFactory
    {
        public static IVectorCollection Create() => new VectorCollection();
    }
}
