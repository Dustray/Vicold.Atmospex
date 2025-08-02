using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.DataService.Provider
{
    public sealed class GridData<TType> : GridData where TType : struct
    {
        public unsafe GridData(int xsize, int ysize)
            : base(xsize, ysize, TypeTool.GetTypeSize<TType>())
        {
            if (!TypeTool.IsNumericType<TType>())
            {
                throw new Exception("类型必须是数值类型。");
            }

            DataType = TypeTool.GetType<TType>();
        }

        public DateTime Now
        {
            get; set;
        }

        public void ReadBlock(int xoffset, int yoffset, IntPtr dst, int xsize, int ysize)
        {
            int srcStride = Width * SizeOfType;
            int dstStride = xsize * SizeOfType;

            int srcIndex = yoffset * srcStride + xoffset * SizeOfType;
            for (int row = 0; row < ysize; ++row)
            {
                var dstData = IntPtr.Add(dst, row * dstStride);
                _memory.Read(srcIndex, dstData, dstStride);
                srcIndex += srcStride;
            }
        }

        public void WriteBlock(int xoffset, int yoffset, int xsize, int ysize, IntPtr data)
        {
            int dstStride = Width * SizeOfType;
            int srcStride = xsize * SizeOfType;

            int dstIndex = yoffset * dstStride + xoffset * SizeOfType;
            IntPtr srcData = data;
            for (int row = 0; row < ysize; ++row)
            {
                _memory.Write(dstIndex, srcData, srcStride);
                dstIndex += dstStride;
                srcData = IntPtr.Add(srcData, srcStride);
            }
        }

        public override GridData DeepCopy()
        {
            GridData newdata = new GridData<TType>(Width, Height);
            this.CopyTo(0, 0, newdata, 0, 0, Width, Height);

            newdata.NoDataValue = this.NoDataValue;
            newdata.MinValue = this.MinValue;
            newdata.MaxValue = this.MaxValue;

            if (this.ValidSubRegion.HasValue)
                newdata.ValidSubRegion = ValidSubRegion.Value;

            return newdata;
        }

    }
}
