using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Algorithm;
using Vicold.Atmospex.DataService.Provider;
using Vicold.Atmospex.FileSystem;

namespace Vicold.Atmospex.Data.Providers
{
    public class RMGridProvider : GridDataProvider
    {
        private GridData[] _gridDatas;

        private FileHost _fileHost;

        public RMGridProvider(FileHost fileHost, DataType dataType)
        {
            _fileHost = fileHost;
            DataType = dataType;
            Count = 1;
        }

        public DateTime FileDate
        {
            get; set;
        }
        public DataType DataType
        {
            get;
        }


        public override GridData GetData()
        {
            return GetData(Index);
        }

        public override GridData GetData(int index)
        {
            if (_gridDatas == null)
            {
                throw new Exception("数据未加载");
            }

            if (index < 0 || index >= _gridDatas.Length)
            {
                throw new Exception("Provider数据越界");
            }

            if (SmoothType != -1)
            {
                return Smooth(_gridDatas[index]); // 平滑
            }
            else
            {
                return _gridDatas[index];
            }
        }

        public bool TryGetData<T>(out GridData<T> gridData) where T : struct
        {
            GridData data = GetData();

            if (data is GridData<T> d)
            {
                gridData = d;
                return true;
            }

            gridData = default;
            return false;
        }


        protected override void DoLoadData()
        {
            _gridDatas = new GridData[1];

            Index = 0;

            if (DataType == DataType.Float32)
            {
                _gridDatas[0] = ReadRM2Float(_fileHost.Source);
            }
            else if (DataType == DataType.Int64)
            {
                _gridDatas[0] = ReadRM3Long(_fileHost.Source);
            }
        }

        private GridData<long> ReadRM3Long(string path)
        {
            var fs = new FileStream(path, FileMode.Open);
            var reader = new BinaryReader(fs);

            StartLongitude = reader.ReadSingle();
            StartLatitude = reader.ReadSingle();
            LonInterval = reader.ReadSingle();
            LatInterval = reader.ReadSingle();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            var gridData = new GridData<long>(Width, Height);

            for (var y = 0; y < gridData.Height; y++)
            {
                for (var x = 0; x < gridData.Width; x++)
                {
                    gridData.WriteDouble(x, y, reader.ReadInt64());
                }
            }
            fs.Dispose();
            reader.Dispose();
            return gridData;
        }

        private GridData<float> ReadRM2Float(string path, bool isSmooth = false)
        {
            var fs = new FileStream(path, FileMode.Open);
            var reader = new BinaryReader(fs);

            StartLongitude = reader.ReadSingle();
            StartLatitude = reader.ReadSingle();
            LonInterval = reader.ReadSingle();
            LatInterval = reader.ReadSingle();
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            var gridData = new GridData<float>(Width, Height);

            for (var y = 0; y < gridData.Height; y++)
            {
                for (var x = 0; x < gridData.Width; x++)
                {
                    gridData.WriteFloat(x, y, reader.ReadSingle());
                }
            }

            fs.Dispose();
            reader.Dispose();
            if (isSmooth)
            {
                IntPtr write = IntPtr.Zero;
                gridData.ScopeLock((ptr) =>
                {
                    write = SmoothAlgorithm.Go(ptr, gridData.Width, gridData.Height, -999);
                });
                gridData.WriteBlock(0, 0, gridData.Width, gridData.Height, write);

            }
            return gridData;
        }


        public override void Dispose()
        {
            if (_gridDatas != null)
            {
                foreach (var grid in _gridDatas)
                {
                    grid.Dispose();
                }
            }

            _gridDatas = null;
        }

        public override DataMessage GetMessage(int index)
        {
            if (index < 0 || index >= _gridDatas.Length)
            {
                throw new Exception("Provider数据越界");
            }

            return new DataMessage()
            {
                ElemName = " ",
                ElemValue = $" ",
            };
        }
    }
}
