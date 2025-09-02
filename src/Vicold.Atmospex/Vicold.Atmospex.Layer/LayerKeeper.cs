using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Vicold.Atmospex.Layer
{
    internal class LayerKeeper
    {
        private readonly Dictionary<string, ILayer> _layerQueue;

        public LayerKeeper()
        {
            _layerQueue = [];
        }

        public bool TryGet(string id, [NotNullWhen(true)] out ILayer? layer)
        {
            return _layerQueue.TryGetValue(id, out layer);
        }

        public void Add(ILayer layer)
        {
            _layerQueue.Add(layer.ID, layer);
        }

        public void Remove(ILayer layer)
        {
            _layerQueue.Remove(layer.ID);
        }

        public IEnumerable<ILayer> GetAllLayers()
        {
            return _layerQueue.Values;
        }

    }
}
