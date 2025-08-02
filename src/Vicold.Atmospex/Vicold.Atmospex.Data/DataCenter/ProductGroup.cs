using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.Data.DataCenter
{
    public class ProductGroup
    {
        /// <summary>
        /// 产品组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据文件格式
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        public List<Product> Products { get; set; }
    }
}
