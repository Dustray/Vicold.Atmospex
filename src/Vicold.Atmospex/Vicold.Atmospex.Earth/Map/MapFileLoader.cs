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

namespace Vicold.Atmospex.GisMap.Map
{
    internal class MapFileLoader
    {
        private static string _folder = Path.GetFullPath(@"data\map\super_low_density");
        //private const string _folder = @"D:\Project\RMIAS\dist\data\map\super_low_density";
        private string _worldContinentControl = $@"{_folder}\WorldContinentBorder.ctl";
        private string _worldContinentData = $@"{_folder}\WorldContinentBorder.adr";
        private string _chinaCoastalControl = $@"{_folder}\ChinaCoastalBorder.ctl";
        private string _chinaCoastalData = $@"{_folder}\ChinaCoastalBorder.adr";
        private string _globalControl = @"J:\Example\TestProjectRepos\projects\Sharp2LiteDB\Sharp2LiteDB\Appx\output\shp2b.ctl";
        private string _globalData = @"J:\Example\TestProjectRepos\projects\Sharp2LiteDB\Sharp2LiteDB\Appx\output\shp2b.adr";

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

        }
        public LineData WorldLines { get; private set; }
        public LineData WorldPolygon { get; private set; }

        public LineData ChinaCoastalLines { get; private set; }

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

            WorldLines = new LineData();
            WorldPolygon = new LineData();
            using (var fs = new FileStream(Path.GetFullPath(dataPath), FileMode.Open))
            {
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
            using (var fs = new FileStream(Path.GetFullPath(dataPath), FileMode.Open))
            {
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
}
