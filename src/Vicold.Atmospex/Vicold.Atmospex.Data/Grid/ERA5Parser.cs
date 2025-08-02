using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.NetCDF4;
using Vicold.Atmospex.Data.Provider.NetCDF.Utilities;

namespace Vicold.Atmospex.DataService.Provider
{
    internal class ERA5Parser
    {
        public ERA5Parser(bool isMerge = false )
        {
            IsMerge = isMerge;
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

        public bool IsMerge { get; set; }

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
                var sName = metaData["Name"].ToString();
                var offset = float.Parse(metaData["add_offset"].ToString());
                var scale = float.Parse(metaData["scale_factor"].ToString());
                var missingValue = float.Parse(metaData["missing_value"].ToString());
                var name = metaData["long_name"].ToString();
                FileDate = NCFileUtility.GetFileDateTime(filePath);
                Name = name;// $"{FileDate:yyyy-MM-dd} {name}";
                ShortName = sName;

                float unitScale = GetUnitScale(sName);
                float unitOffset = GetUnitOffset(sName);
                // 生成GridData
                GridData[] gridDatas;
                if (IsMerge )
                {
                    gridDatas = new GridData[1];
                    var gridData1 = new GridData<float>(lonCount, latCount);
                    gridData1.NoDataValue = 9999f;
                    var newData = new float[timeCount, lonCount];
                    if (vData.TypeOfData == typeof(short))
                    {
                        short[,,] data = ve as short[,,];

                        //for (var y = 0; y < latCount; y++)
                        Parallel.For(0, latCount, (y) =>
                        {
                            for (var x = 0; x < lonCount; x++)
                            {
                                var value = 9999f;
                                for (var i = 0; i < timeCount; i++)
                                {
                                    float v = data[i, y, x];
                                    if (v == missingValue)
                                    {
                                        continue;
                                    }
                                    else if (value == 9999f && v != missingValue)
                                    {
                                        value = 0;
                                    }

                                    value += (v * scale + offset) * unitScale + unitOffset;
                                }

                                gridData1.WriteFloat(x, y, value); // K转摄氏度
                            }
                        });
                        gridDatas[0] = gridData1;
                    }
                }
                else
                {
                    gridDatas = new GridData[timeCount];
                    if (vData.TypeOfData == typeof(short))
                    {
                        short[,,] data = ve as short[,,];

                        Parallel.For(0, timeCount, (i) =>
                        //for (var i = 0; i < timeCount; i++)
                        {
                            var gridData1 = new GridData<float>(lonCount, latCount);
                            gridData1.NoDataValue = 9999f;
                            for (var y = 0; y < latCount; y++)
                            {
                                for (var x = 0; x < lonCount; x++)
                                {
                                    float value = data[i, y, x];
                                    if (value == missingValue)
                                    {
                                        value = 9999f;
                                    }
                                    else
                                    {
                                        value = (value * scale + offset) * unitScale + unitOffset;
                                    }

                                    gridData1.WriteFloat(x, y, value); // K转摄氏度
                                                                       //Godot.GD.Print((data[i, y, x] * scale + offset + unitOffset));
                                }
                            }
                            //var gridData = GetGridData<short>(lonCount, latCount, data);
                            gridDatas[i] = gridData1;
                        });
                    }
                }

                return gridDatas;
            }

        }

        private float GetUnitOffset(string name)
        {
            if (name == "t" || name == "t2m")
            {
                return -273.15f;
            }

            return 0;
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
