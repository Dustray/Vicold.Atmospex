using System;
using System.Collections.Generic;
using System.Text;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.DataCenter;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.DataService.Provider;
using Vicold.Atmospex.FileSystem;

namespace Vicold.Atmospex.Data.Provider.NetCDF
{
    public class NetCDFProvider : GridDataProvider, IFileBoot
    {
        private ERA5Parser _netCDFParser;

        private GridData[] _gridDatas;

        public NetCDFProvider(string filePath) : this(new FileHost(filePath, new NetCDFFileCatalog(filePath)))
        {
        }

        public NetCDFProvider(FileHost fileHost)
        {
            FileHost = fileHost;
            //FileHost.OnSourceUpdated = OnSourceUpdated;
            if (DataModuleService.Current is { })
            {
                _netCDFParser = new ERA5Parser(DataModuleService.Current.Productor);
            }

        }

        public FileHost FileHost
        {
            get;
        }

        public DateTime FileDate
        {
            get; set;
        }

        public Product Product => _netCDFParser.Product;

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

        protected override void DoLoadData()
        {
            _gridDatas = _netCDFParser.Parse(FileHost.Source);

            Width = _netCDFParser.Width;
            Height = _netCDFParser.Height;
            LonInterval = _netCDFParser.LonInterval;
            LatInterval = _netCDFParser.LatInterval;
            StartLongitude = _netCDFParser.StartLongitude;
            StartLatitude = _netCDFParser.StartLatitude;
            Name = _netCDFParser.Name;
            Key = _netCDFParser.ShortName;
            FileDate = _netCDFParser.FileDate;
            Count = _gridDatas.Length;
            Index = 0;

            //Godot.GD.Print(_product?.Display??"ds");
            RefreshName();
        }

        public void RefreshName()
        {
            Name = $"{FileDate:yyyy-MM-dd} {Product?.Display ?? _netCDFParser.Name}";
        }

        public override DataMessage GetMessage(int index)
        {
            if (index < 0 || index >= _gridDatas.Length)
            {
                throw new Exception("Provider数据越界");
            }

            return new DataMessage()
            {
                ElemName = "时刻",
                ElemValue = $"{index}时（UTC）",
            };
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

    }
}
