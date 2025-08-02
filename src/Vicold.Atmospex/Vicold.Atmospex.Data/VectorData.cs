using Vicold.Atmospex.Data.Vector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vicold.Atmospex.Data
{
    public class VectorData : IVectorData
    {
        private IVectorCollection _vectorCollection;

        public VectorData()
        {
            _vectorCollection = VectorCollectionFactory.Create();
        }

        public IVectorCollection Collection => _vectorCollection;

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _vectorCollection.Dispose();
        }
    }
}
