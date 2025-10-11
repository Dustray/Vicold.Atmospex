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

        public void RollLongitude(double centerLongitude)
        {
            // 计算当前中心经度
            double currentCenterLongitude = (StartLongitude + EndLongitude) / 2;
            
            // 判断当前中心经度是否与传入参数一致，一致则直接跳过
            if (Math.Abs(currentCenterLongitude - centerLongitude) < 1e-10)
            {
                return;
            }
            
            // 计算需要滚动的经度偏移量
            double offset = centerLongitude - currentCenterLongitude;
            
            // 更新起始经度
            StartLongitude += offset;
            
            // 同时更新结束经度，保持经度范围一致
            
            // 确保经度值在合理范围内（-180 到 180 度）
            while (StartLongitude < -180)
            {
                StartLongitude += 360;
            }
            while (StartLongitude > 180)
            {
                StartLongitude -= 360;
            }
            
            // 计算需要滚动的列数
            double totalLongitudeRange = EndLongitude - StartLongitude;
            if (totalLongitudeRange < 0) totalLongitudeRange += 360; // 处理跨 180 度的情况
            
            int shiftColumns = (int)Math.Round((offset / totalLongitudeRange) * Width);
            
            // 处理 shiftColumns 可能为负值或超出宽度的情况
            shiftColumns = ((shiftColumns % Width) + Width) % Width;
            
            // 如果不需要滚动，直接返回
            if (shiftColumns == 0 || !IsLoaded)
            {
                return;
            }
            
            // 获取所有的 GridData 并重新组织其内存数据
            for (int i = 0; i < Count; i++)
            {
                GridData gridData = GetData(i);
                if (gridData != null)
                {
                    // 创建一个临时的缓冲区来存储重新组织后的数据
                    int totalSize = gridData.Width * gridData.Height * gridData.SizeOfType;
                    byte[] newData = new byte[totalSize];
                    
                    // 计算每行的数据大小
                    int rowSize = gridData.Width * gridData.SizeOfType;
                    int shiftedRowSize = shiftColumns * gridData.SizeOfType;
                    int remainingRowSize = rowSize - shiftedRowSize;
                    
                    // 第一步：从原始数据读取并重新组织数据到 newData 数组
                    gridData.ScopeLock(ptr =>
                    {
                        for (int y = 0; y < gridData.Height; y++)
                        {
                            // 计算源数据和目标数据的起始位置
                            int srcRowStart = y * rowSize;
                            int dstRowStart = y * rowSize;
                            
                            // 复制后半部分数据到新数据的前半部分
                            System.Runtime.InteropServices.Marshal.Copy(
                                new IntPtr(ptr.ToInt64() + srcRowStart + remainingRowSize),
                                newData,
                                dstRowStart,
                                shiftedRowSize);
                            
                            // 复制前半部分数据到新数据的后半部分
                            System.Runtime.InteropServices.Marshal.Copy(
                                new IntPtr(ptr.ToInt64() + srcRowStart),
                                newData,
                                dstRowStart + shiftedRowSize,
                                remainingRowSize);
                        }
                    });
                    
                    // 第二步：将重新组织的数据写回到 gridData 中
                    // 使用 ScopeLock 来获取直接内存访问权限
                    gridData.ScopeLock(ptr =>
                    {
                        // 将整个 newData 数组复制到 gridData 的内存中
                        System.Runtime.InteropServices.Marshal.Copy(newData, 0, ptr, totalSize);
                    });
                }
            }
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
