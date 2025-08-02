using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Style.JsonEtt
{
    internal class JsonPalette
    {
        public string Name { get; set; }

        public bool AutoValue { get; set; } = true;

        public string RenderType { get; set; } = "Bitmap";

        public IList<JsonValueColor> Colors { get; set; }

        public float[] ContourAnaValues { get; set; }
        public bool UseAnaValues { get; set; }
    }
}
