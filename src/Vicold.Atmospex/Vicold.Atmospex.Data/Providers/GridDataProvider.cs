using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Algorithm;
using Vicold.Atmospex.DataService.Provider;

namespace Vicold.Atmospex.Data.Providers
{
    public abstract class GridDataProvider : IGridDataProvider
    {
        public bool IsLoaded { get; set; } = false;

        public int Count
        {
            get; protected set;
        }

        public int Width
        {
            get; protected set;
        }

        public int Height
        {
            get; protected set;
        }

        public double LonInterval
        {
            get; protected set;
        }

        public double LatInterval
        {
            get; protected set;
        }

        public double StartLongitude
        {
            get; protected set;
        }

        public double StartLatitude
        {
            get; protected set;
        }

        public double EndLongitude => StartLongitude + Width * LonInterval;

        public double EndLatitude => StartLatitude + Height * LatInterval;

        public int Level
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Key
        {
            get; set;
        }

        public int Index
        {
            get; set;
        }

        public int SmoothType { get; set; } = -1;

        public abstract void Dispose();


        /// <summary>
        /// 获取默认GridData
        /// </summary>
        /// <returns></returns>
        public abstract GridData GetData();

        public abstract GridData GetData(int index);


        public void SmoothRoll()
        {
            SmoothType++;
            if (SmoothType > 2)
            {
                SmoothType = -1;
            }
        }


        protected GridData Smooth(GridData src)
        {
            var dst = new GridData<float>(src.Width, src.Height);
            IntPtr write = IntPtr.Zero;
            src.ScopeLock((ptr) =>
            {
                write = SmoothAlgorithm.Go2(ptr, src.Width, src.Height, -999, SmoothType);
            });
            dst.WriteBlock(0, 0, src.Width, src.Height, write);
            return dst;
        }

        public void LoadData()
        {
            if (IsLoaded)
            {
                return;
            }

            DoLoadData();
            IsLoaded = true;
        }
        public void ReLoadData()
        {
            IsLoaded = false;
            LoadData();
        }

        protected abstract void DoLoadData();

        public abstract DataMessage GetMessage(int index);
    }
}
