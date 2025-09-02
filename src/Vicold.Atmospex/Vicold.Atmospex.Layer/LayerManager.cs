using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Configration;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Layer.Events;
using Vicold.Atmospex.Layer.Tool;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Layer
{
    internal class LayerManager : ILayerManager
    {
        //private IBus _globalBus;
        private LayerKeeper _layerKeeper;
        private IProjection _projection;

        //private VisionGate _vision => Launcher.Current.VisionBinding;

        public LayerManager()
        {
            _layerKeeper = new LayerKeeper();
        }

        public event LayerChangedEventHandler OnLayerAdded;

        public event LayerChangedEventHandler OnLayerRemoved;

        public event LayerChangedEventHandler OnLayerUpdated;

        public event LayerChangedEventHandler OnLayerVisibleChanged;

        //public void LoadBus(IBus globalBus) => _globalBus = globalBus;



        public bool TryGetLayer(string id, out ILayer layer)
        {
            return _layerKeeper.TryGet(id, out layer);
        }

        public void AddLayer(ILayer layer)
        {

            if (layer is GridLayer && layer.Style == null)
            {
                var ss = LayerModuleService.GetService<IConfigModuleService>();
                if (ss is { })
                {
                    layer.Style = StyleHelper.GetDefaultPaletteStyle(ss);
                }
            }

            var earthService = LayerModuleService.GetService<IEarthModuleService>();
            if (earthService == null || earthService.CurrentProjection == null)
            {
                throw new Exception("未找到EarthService");
            }

            layer.Render(earthService.CurrentProjection);
            var node = LayerExtractor.ExtractLayerNode(layer);
            if (node != null)
            {
                if (_layerKeeper.TryGet(layer.ID, out var oldLayer))
                {
                    var oldNode = LayerExtractor.ExtractLayerNode(layer);
                    //_vision.OnNodeRemove.Invoke(oldNode);
                }

                //_vision.OnNodeLoad.Invoke(node);
                OnLayerAdded?.Invoke(this, CreateArgs(layer));
                _layerKeeper.Add(layer);
            }
        }

        public void RemoveLayer(ILayer layer)
        {
            var node = LayerExtractor.ExtractLayerNode(layer);
            if (node != null)
            {
                //_vision.OnNodeRemove.Invoke(node);
            }

            _layerKeeper.Remove(layer);
            OnLayerRemoved?.Invoke(this, CreateArgs(layer));
            layer.Dispose();
            //layer = null;
            //GC.Collect(); // 主动GC
        }

        public void UpdateLayer(ILayer layer)
        {
            if (!_layerKeeper.TryGet(layer.ID, out _))
            {
                return;
            }

            if (layer is GridLayer && layer.Style == null)
            {
                var ss = LayerModuleService.GetService<IConfigModuleService>();
                if (ss is { })
                {
                    layer.Style = StyleHelper.GetDefaultPaletteStyle(ss);
                }
            }

            layer.Render(_projection);
            var node = LayerExtractor.ExtractLayerNode(layer);
            if (node != null)
            {
                //_vision.OnNodeLoad.Invoke(node);
                OnLayerUpdated?.Invoke(this, CreateArgs(layer));
            }
        }

        public void SetLayerLevel(ILayer layer, LayerLevel layerLevel)
        {

        }

        public void SetLayerVisible(ILayer layer, bool isVisible)
        {
            var node = LayerExtractor.ExtractLayerNode(layer);
            if (node != null)
            {
                //_vision.OnNodeVisibleChanged.Invoke(node, isVisible);
            }

            OnLayerVisibleChanged?.Invoke(this, CreateArgs(layer));
        }

        public IEnumerable<ILayer> GetAllLayers()
        {
            return _layerKeeper.GetAllLayers();
        }

        private LayerChangedEventArgs CreateArgs(ILayer layer)
        {
            return new LayerChangedEventArgs(layer);
        }

    }
}
