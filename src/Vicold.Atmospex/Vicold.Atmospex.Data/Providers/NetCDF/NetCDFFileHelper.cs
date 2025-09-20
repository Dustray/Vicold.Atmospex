using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.NetCDF4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data.Provider.NetCDF
{
    public static class NetCDFFileHelper
    {
        public static bool IsNetCDFFile(string filePath)
        {
            try
            {
                using (_ = new NetCDFDataSet(filePath, ResourceOpenMode.Open))
                {
                }
                return true;
            }
            catch (Exception e)
            {
                //Godot.GD.Print(e);
                return false;
            }
        }
    }
}
