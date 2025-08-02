using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data.Provider.NetCDF.Utilities
{
    public static class UnitChangeUtility
    {
        public static float K2C(float value)
        {
            return value - 273.15f;
        }

        /// <summary>
        /// 水柱高h=Soil（kg/m2）/密度(ρ)，水密度为1，所以等效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static float kgm2_mm(float value)
        {
            return 0;
        }
    }
}
