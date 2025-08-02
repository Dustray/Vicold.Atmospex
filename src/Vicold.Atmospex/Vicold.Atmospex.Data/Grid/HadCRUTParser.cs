using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.NetCDF4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Data.Provider.NetCDF.Utilities;

namespace Vicold.Atmospex.DataService.Provider
{
    internal class HadCRUTParser
    {
        public HadCRUTParser()
        {
        }


        public string ShortName { get; set; }

        public string Name { get; set; }

        public int Count { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Level { get; private set; } = 1;

        public double LonInterval { get; private set; }

        public double LatInterval { get; private set; }

        public double StartLongitude { get; private set; }

        public double StartLatitude { get; private set; }

        public double EndLongitude { get; private set; }

        public double EndLatitude { get; private set; }

        public DateTime FileDate { get; set; }

        private int[][] indexes = new int[][]{
            new int[] { 0, 1, 2, 3, 4, 6, 8, 9},
            new int[] { 1, 0, 2, 3, 2, 4, 6, 7},
            new int[] { 1, 0, 2, 3, 2, 4, 6, 7},
};

        public GridData[] Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            var filePath2 = Path.GetFileNameWithoutExtension(filePath);
            //GridData[] gridDatas;
            int[] pp;
            if (filePath2.StartsWith("HadCRUT"))
            {
                pp = indexes[0];
            }
            else if (filePath2.StartsWith("CRUTEM"))
            {
                pp = indexes[1];
            }
            else
            {
                pp = indexes[2];
            }
            using (NetCDFDataSet ds = new NetCDFDataSet(filePath, ResourceOpenMode.Open))
            {
                // 获取维度长度
                var dimen = ds.Dimensions;
                var dimenCount = dimen.Count; // 维度个数
                var bndsCount = dimen[pp[0]].Length;
                var lonCount = dimen[pp[1]].Length;
                var latCount = dimen[pp[2]].Length;
                var timeCount = dimen[pp[3]].Length;
                Count = timeCount;

                // 要素
                var vLon = ds.Variables[pp[4]];
                var vLat = ds.Variables[pp[5]];
                var vTime = ds.Variables[pp[6]];

                // 经纬度数据
                var lonData = vLon.GetData() as double[];
                var latData = vLat.GetData() as double[];
                Width = lonData.Length;
                Height = latData.Length;

                // 计算起始经纬度和格距
                var lonStart = lonData[0];
                var lonEnd = lonData[lonData.Length - 1];
                var latStart = latData[0];
                var latEnd = latData[latData.Length - 1];
                var lonInterval = (lonEnd - lonStart) / (lonData.Length - 1);
                var latInterval = (latEnd - latStart) / (latData.Length - 1);
                LonInterval = lonInterval;
                LatInterval = latInterval;
                StartLongitude = lonStart;
                StartLatitude = latStart;

                // 数据要素
                var vData = ds.Variables[pp[7]];
                var ve = vData.GetData();

                // 各种信息
                var metaData = vData.Metadata;
                var sName = metaData["Name"].ToString();
                var missingValue = double.Parse(metaData["_FillValue"].ToString());
                var name = metaData["long_name"].ToString();
                FileDate = NCFileUtility.GetFileDateTime(filePath);
                Name = name;// $"{FileDate:yyyy-MM-dd} {name}";
                ShortName = sName;

                float unitScale = GetUnitScale(sName);
                float unitOffset = GetUnitOffset(sName);
                // 生成GridData
                GridData[] gridDatas;
                var startDate = new DateTime(1850, 1, 1);
                gridDatas = new GridData[timeCount];
                if (vData.TypeOfData == typeof(double))
                {
                    double[,,] data = ve as double[,,];

                    Parallel.For(0, timeCount, (i) =>
                    //for (var i = 0; i < timeCount; i++)
                    {
                        var gridData1 = new GridData<float>(lonCount, latCount);
                        gridData1.NoDataValue = missingValue;
                        gridData1.Now = startDate.AddMonths(i);
                        for (var y = 0; y < latCount; y++)
                        {
                            for (var x = 0; x < lonCount; x++)
                            {
                                float value = (float)data[i, y, x];
                                if (value == missingValue)
                                {
                                    value = 9999f;
                                }
                                else
                                {
                                    value = value * unitScale + unitOffset;
                                }

                                gridData1.WriteFloat(x, y, value); // K转摄氏度
                                                                   //Godot.GD.Print((data[i, y, x] * scale + offset + unitOffset));
                            }
                        }
                        //var gridData = GetGridData<short>(lonCount, latCount, data);
                        gridDatas[i] = gridData1;
                    });
                }else if (vData.TypeOfData == typeof(float))
                {
                    float[,,] data = ve as float[,,];

                    Parallel.For(0, timeCount, (i) =>
                    //for (var i = 0; i < timeCount; i++)
                    {
                        var gridData1 = new GridData<float>(lonCount, latCount);
                        gridData1.NoDataValue = missingValue;
                        gridData1.Now = startDate.AddMonths(i);
                        for (var y = 0; y < latCount; y++)
                        {
                            for (var x = 0; x < lonCount; x++)
                            {
                                float value =data[i, y, x];
                                if (value == missingValue)
                                {
                                    value = 9999f;
                                }
                                else
                                {
                                    value = value * unitScale + unitOffset;
                                }

                                gridData1.WriteFloat(x, y, value); // K转摄氏度
                                                                   //Godot.GD.Print((data[i, y, x] * scale + offset + unitOffset));
                            }
                        }
                        //var gridData = GetGridData<short>(lonCount, latCount, data);
                        gridDatas[i] = gridData1;
                    });
                }

                return gridDatas;
            }

        }

        private float GetUnitOffset(string name)
        {
            return 0;
            //if (name == "tas_mean")
            //{
            //    return -273.15f;
            //}

            //return 0;
        }

        private float GetUnitScale(string name)
        {
            if (name == "crwc")
            {
                return 1293;// 1000/0.773; kg/kg => g/(m^3)
            }
            else if (name == "tp")
            {
                return 1000f;// m->mm
            }
            return 1;
        }

        //private GridData<TType> GetGridData<TType>(int width, int height, TType[,,] data)
        //{
        //    var gridData = new GridData<TType>(width, height, DataType.Int32);


        //    var dd = data[0, 0, 0];
        //    var ff = (short)dd * 8;
        //    return gridData;
        //}
    }
}
