using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.NetCDF4;
using Vicold.Atmospex.Data.Provider.NetCDF.Utilities;
using Vicold.Atmospex.DataService.Provider;

namespace Vicold.Atmospex.Data.Provider.NetCDF
{
    internal class ReAnaRainParser
    {
        public ReAnaRainParser()
        {
        }

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


        public GridData[] Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            //GridData[] gridDatas;
            
            using (NetCDFDataSet ds = new NetCDFDataSet(filePath, ResourceOpenMode.Open))
            {
                // 获取维度长度
                var dimen = ds.Dimensions;
                var dimenCount = dimen.Count; // 维度个数
                var timeCount = dimen[0].Length;
                var latCount = dimen[1].Length;
                var lonCount = dimen[2].Length;
                Count = timeCount;

                // 要素
                var vTime = ds.Variables[2];
                var vLat = ds.Variables[3];
                var vLon = ds.Variables[4];

                // 经纬度数据
                var lonData = vLon.GetData() as float[];
                var latData = vLat.GetData() as float[];
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
                var vData = ds.Variables[1];
                var ve = vData.GetData();

                // 各种信息
                var metaData = vData.Metadata;
                var offset = float.Parse(metaData["add_offset"].ToString());
                var scale = float.Parse(metaData["scale_factor"].ToString());
                var missingValue = float.Parse(metaData["missing_value"].ToString());
                var name = metaData["standard_name"].ToString();
                Name = $"{NCFileUtility.GetFileDateTime(filePath):yyyy-MM-dd} {name}";

               // Godot.GD.Print(scale + ";;;" + offset);

                // 生成GridData
                var gridDatas = new GridData[timeCount];
                Parallel.For(0, timeCount, (i) =>
                //for (var i = 0; i < timeCount; i++)
                {
                    if (vData.TypeOfData == typeof(short))
                    {
                        short[,,] data = ve as short[,,];

                        var gridData1 = new GridData<float>(lonCount, latCount);
                        for (var x = 0; x < lonCount; x++)
                        {
                            for (var y = 0; y < latCount; y++)
                            {
                                gridData1.WriteFloat(x, y, (data[i, y, x] * scale + offset - 273.15f)); // K转摄氏度
                            }
                        }
                        //var gridData = GetGridData<short>(lonCount, latCount, data);
                        gridDatas[i] = gridData1;
                    }
                });

                return gridDatas;
            }
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
