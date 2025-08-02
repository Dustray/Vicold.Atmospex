using Vicold.Atmospex.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Layer.Events
{
    public delegate void LayerChangedEventHandler(object sender, LayerChangedEventArgs e);

    public class LayerChangedEventArgs
    {
        public LayerChangedEventArgs(ILayer layer)
        {
            Layer = layer;
        }

        public ILayer Layer { get; private set; }

        public string LayerId => Layer.ID;

        public bool CanFlip => Layer.DataProvider == null ? false : Layer.DataProvider is IFileBoot;

        public string LayerName => Layer.Name;

        public bool IsVisible => Layer.IsVisible;

        public bool IsSystemLayer => Layer.IsSystemLayer;

    }
}
