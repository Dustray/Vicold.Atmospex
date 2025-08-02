using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vicold.Atmospex.Data.Provider.NetCDF.Utilities
{
    internal static class NCFileUtility
    {
        public static DateTime GetFileDateTime(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            if (name.Length == 8)
            {
                return DateTime.ParseExact(name, "yyyyMMdd", null);
            }
            else if (name.Length == 10)
            {
                return DateTime.ParseExact(name, "yyyyMMddHH", null);
            }
            else return DateTime.Now;
        }
    }
}
