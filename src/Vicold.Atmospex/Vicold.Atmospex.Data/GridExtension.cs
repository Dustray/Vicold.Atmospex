using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.DataService.Provider
{
    public static class GridExtension
    {
        public static void SaveFileFloat<T>(this GridData<T> gridData, string path,
            float startLon, float startLat, float lonInterval, float latInterval) where T : struct
        {
            using var fs = new FileStream(path, FileMode.Create);
            using var writer = new BinaryWriter(fs);

            writer.Write(startLon);
            writer.Write(startLat);
            writer.Write(lonInterval);
            writer.Write(latInterval);
            writer.Write(gridData.Width);
            writer.Write(gridData.Height);

            for (var y = 0; y < gridData.Height; y++)
            {
                for (var x = 0; x < gridData.Width; x++)
                {
                    writer.Write(gridData.ReadFloat(x, y));
                }
            }
        }

       
    }
}
