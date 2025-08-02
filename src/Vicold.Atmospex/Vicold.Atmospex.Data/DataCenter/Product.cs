using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data.DataCenter
{
    public class Product
    {
        /// <summary>
        /// 数据展示名称
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// 数据Key 唯一
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 数据相对路径
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// 样式Key
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// 是否将文件中的所有数据融合为一个
        /// </summary>
        public bool IsMerge { get; set; } = false;
    }
}
