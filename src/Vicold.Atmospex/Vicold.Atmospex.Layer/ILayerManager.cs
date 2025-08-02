using Vicold.Atmospex.Layer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Layer
{
    public interface ILayerManager 
    {
        bool TryGetLayer(string id, out ILayer layer);

        void AddLayer(ILayer layer);

        void RemoveLayer(ILayer layer);

        void UpdateLayer(ILayer layer);

        void SetLayerVisible(ILayer layer, bool isVisible);

        void SetLayerLevel(ILayer layer, LayerLevel layerLevel);

        IEnumerable<ILayer> GetAllLayers();

        event LayerChangedEventHandler OnLayerAdded;

        event LayerChangedEventHandler OnLayerRemoved;

        event LayerChangedEventHandler OnLayerUpdated;

        event LayerChangedEventHandler OnLayerVisibleChanged;

    }
}
