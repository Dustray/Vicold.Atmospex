using Newtonsoft.Json;
using Vicold.Atmospex.Style.JsonEtt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vicold.Atmospex.Style
{
    public static class PaletteHelper
    {
        /// <summary>
        /// 获取调色板实体
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ColorPalette GetColorPalette(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var palette = ReadColorPalette(filePath);

            if (palette == null || palette.Colors.Count == 0)
            {
                return null;
            }

            var colorPalette = new ColorPalette(palette.Colors.Count, palette.Name)
            {
                AutoValue = palette.AutoValue,
                ContourAnaValues = palette.ContourAnaValues,
                UseAnaValues = palette.UseAnaValues,
            };
            switch (palette.RenderType)
            {
                case "Contour":
                    colorPalette.RenderType = RenderType.Contour;
                    break;
                case "Bitmap":
                    colorPalette.RenderType = RenderType.Bitmap;
                    break;
            }

            int index = 0;
            foreach (var p in palette.Colors)
            {
                var rgbaArray = p.RGBA.Split(',');
                if (rgbaArray.Length < 3)// 可以是rbg或者rgba，超过4个只取前三个
                {
                    throw new Exception($"调色板（{filePath}）中第{index}个值颜色不正确");
                }

                var rr = byte.TryParse(rgbaArray[0], out var r);
                var gr = byte.TryParse(rgbaArray[1], out var g);
                var br = byte.TryParse(rgbaArray[2], out var b);

                byte a = 255;
                bool ar = true;
                if (rgbaArray.Length > 3)
                {
                    ar = byte.TryParse(rgbaArray[3], out a);
                }

                if (!(rr && gr && br && ar))
                {
                    throw new Exception($"调色板（{filePath}）中第{index}项颜色不正确");
                }

                colorPalette.Set(index, new ColorItem(p.Value, r, g, b, a));
                index++;
            }

            return colorPalette;
        }

        /// <summary>
        /// 读取调色板文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static JsonPalette ReadColorPalette(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = File.ReadAllText(filePath);
            try
            {
                var palette = JsonConvert.DeserializeObject<JsonPalette>(json);
                return palette;
            }
            catch (Exception e)
            {

                return null;
            }
        }
    }
}
