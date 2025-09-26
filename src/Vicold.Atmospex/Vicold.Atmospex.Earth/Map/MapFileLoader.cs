using Newtonsoft.Json;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Vector;
using Vicold.Atmospex.Earth.Projection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Earth.Map;

internal class MapFileLoader
{
    private static readonly string _mapFolder = Path.GetFullPath(@"data\map");
    private static readonly string _folder = Path.GetFullPath(@"data\map\super_low_density");
    //private const string _folder = @"D:\Project\Vicold.Atmospex\dist\data\map\super_low_density";
    private readonly string _worldContinentControl = $@"{_folder}\WorldContinentBorder.ctl";
    private readonly string _worldContinentData = $@"{_folder}\WorldContinentBorder.adr";
    private readonly string _chinaCoastalControl = $@"{_folder}\ChinaCoastalBorder.ctl";
    private readonly string _chinaCoastalData = $@"{_folder}\ChinaCoastalBorder.adr";
    private readonly string _globalControl = @"J:\Example\TestProjectRepos\projects\Sharp2LiteDB\Sharp2LiteDB\Appx\output\shp2b.ctl";
    private readonly string _globalData = @"J:\Example\TestProjectRepos\projects\Sharp2LiteDB\Sharp2LiteDB\Appx\output\shp2b.adr";
    private readonly string _chinaProvinceControl = $@"{_folder}\ChinaProvinceBorder.ctl";
    private readonly string _chinaProvinceData = $@"{_folder}\ChinaProvinceBorder.adr";
    private readonly string _chinaProvinceSape = $@"{_mapFolder}\source\中国_省.geojson";

    public MapFileLoader()
    {
        //_folder = $@"{workSpace}\data\map\super_low_density";
    }

    public void LoadData()
    {
        //var lineDic = LoadAdrShape(_globalControl, _globalData);
        //Lines = LoadToMap(lineDic);
        if (!Directory.Exists(_folder))
        {
            return;
        }

        LoadAdrShapeLinePolygon(_worldContinentControl, _worldContinentData, 1, Color.FromArgb(100, 100, 100), Color.White);
        ChinaCoastalLines = LoadAdrShape(_chinaCoastalControl, _chinaCoastalData, 1, Color.Black, Color.White);

        if (!File.Exists(_chinaProvinceControl) || !File.Exists(_chinaProvinceData))
        {
            WriteControlAndDataShape(_chinaProvinceSape, _chinaProvinceControl, _chinaProvinceData);
        }
        ChinaProvinceLines = LoadAdrShape(_chinaProvinceControl, _chinaProvinceData, 1, Color.Black, Color.White);

    }

    private void WriteControlAndDataShape(string geojsonPath, string controlPath, string dataPath)
    {
        try
        {
            // 确保输出目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(controlPath));
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));

            // 读取GeoJSON文件
            string geojsonContent = File.ReadAllText(Path.GetFullPath(geojsonPath));
            dynamic geoJson = JsonConvert.DeserializeObject(geojsonContent);

            // 准备控制数据和二进制数据
            var controlData = new Dictionary<int, List<(int start, int length)>>();
            using var dataStream = new FileStream(Path.GetFullPath(dataPath), FileMode.Create);
            int featureId = 1; // 简单起见，使用递增ID

            // 处理GeoJSON中的features
            foreach (var feature in geoJson.features)
            {
                var coordinates = feature.geometry.coordinates;
                var ranges = new List<(int start, int length)>();

                // 处理不同类型的几何图形
                string geometryType = feature.geometry.type.ToString();
                if (geometryType == "Polygon")
                {
                    // 处理单个多边形
                    ProcessPolygon(coordinates, featureId, ranges, dataStream);
                }
                else if (geometryType == "MultiPolygon")
                {
                    // 处理多个多边形
                    foreach (var polygon in coordinates)
                    {
                        ProcessPolygon(polygon, featureId, ranges, dataStream);
                    }
                }

                controlData[featureId] = ranges;
                featureId++;
            }

            // 保存控制文件
            string controlJson = JsonConvert.SerializeObject(controlData);
            File.WriteAllText(Path.GetFullPath(controlPath), controlJson);
        }
        catch (Exception ex)
        {
            // 记录异常信息
            Console.WriteLine($"写入地图文件时出错: {ex.Message}");
        }
    }

    private void ProcessPolygon(dynamic polygonCoordinates, int featureId, List<(int start, int length)> ranges, FileStream dataStream)
    {
        // 每个多边形可能有多个环（外环和内环）
        foreach (var ring in polygonCoordinates)
        {
            int pointCount = 0;
            // 计算点数
            foreach (var coord in ring)
            {
                pointCount++;
            }

            // 准备坐标数据缓冲区
            byte[] buffer = new byte[pointCount * 16]; // 每个点包含经度和纬度，每个是8字节(double)

            int bufferIndex = 0;
            // 将坐标转换为二进制数据
            foreach (var coord in ring)
            {
                double longitude = coord[0];
                double latitude = coord[1];

                // 写入经度
                byte[] lonBytes = BitConverter.GetBytes(longitude);
                Array.Copy(lonBytes, 0, buffer, bufferIndex, 8);
                bufferIndex += 8;

                // 写入纬度
                byte[] latBytes = BitConverter.GetBytes(latitude);
                Array.Copy(latBytes, 0, buffer, bufferIndex, 8);
                bufferIndex += 8;
            }

            // 记录起始位置和长度
            int startPosition = (int)dataStream.Position;
            int length = buffer.Length;
            ranges.Add((startPosition, length));

            // 写入数据
            dataStream.Write(buffer, 0, buffer.Length);
        }
    }

    public LineData? WorldLines
    {
        get; private set;
    }

    public LineData? WorldPolygon
    {
        get; private set;
    }

    public LineData? ChinaCoastalLines
    {
        get; private set;
    }

    public LineData? ChinaProvinceLines
    {
        get;
        internal set;
    }

    /// <summary>
    /// 从二进制文件加载shape数据
    /// </summary>
    /// <param name="ctlPath">控制文件路径</param>
    /// <param name="dataPath">数据文件路径</param>
    /// <returns></returns>
    private void LoadAdrShapeLinePolygon(string ctlPath, string dataPath, float width, Color color, Color fillColor)
    {
        var ctlFile = File.ReadAllText(Path.GetFullPath(ctlPath));
        var ctlContent = JsonConvert.DeserializeObject<Dictionary<int, IList<(int start, int length)>>>(ctlFile);
        if (ctlContent is { })
        {
            WorldLines = new LineData();
            WorldPolygon = new LineData();
            using var fs = new FileStream(Path.GetFullPath(dataPath), FileMode.Open);
            foreach (var code in ctlContent)
            {
                var rangePositions = code.Value;
                foreach (var position in rangePositions)
                {
                    fs.Seek(position.start, SeekOrigin.Begin);
                    var b = new byte[position.length];
                    fs.Read(b, 0, position.length);

                    var dataArray = new float[position.length / 8];
                    for (int i = 0, j = 0; i < b.Length; i += 8, j++)
                    {
                        dataArray[j] = (float)BitConverter.ToDouble(b, i);
                    }

                    WorldLines.Add(dataArray, width, 0, color, fillColor, PolygonType.Line);
                    WorldPolygon.Add(dataArray, width, 0, color, fillColor, PolygonType.Fill);
                }
            }
        }
    }

    private static LineData LoadAdrShape(string ctlPath, string dataPath, float width, Color color, Color fillColor, PolygonType polygonType = PolygonType.Line)
    {
        var ctlFile = File.ReadAllText(Path.GetFullPath(ctlPath));
        var ctlContent = JsonConvert.DeserializeObject<Dictionary<int, IList<(int start, int length)>>>(ctlFile);
        var result = new LineData();
        if (ctlContent is { })
        {
            using var fs = new FileStream(Path.GetFullPath(dataPath), FileMode.Open);
            foreach (var code in ctlContent)
            {
                var rangePositions = code.Value;
                foreach (var position in rangePositions)
                {
                    fs.Seek(position.start, SeekOrigin.Begin);
                    var b = new byte[position.length];
                    fs.Read(b, 0, position.length);

                    var dataArray = new float[position.length / 8];
                    for (int i = 0, j = 0; i < b.Length; i += 8, j++)
                    {
                        dataArray[j] = (float)BitConverter.ToDouble(b, i);
                    }

                    result.Add(dataArray, width, 0, color, fillColor, polygonType);
                }
            }
        }

        return result;
    }
}
