using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vicold.Atmospex.Configration;

namespace Vicold.Atmospex.Data.DataCenter
{
    public class ProductKeeper
    {
        private List<ProductGroup> _groups = new List<ProductGroup>();

        public ProductKeeper(string workSpace)
        {
            Init(workSpace);
        }

        private async void Init(string workSpace)
        {
            var productDir = Path.Combine(workSpace, @"config\product");
            if (Directory.Exists(productDir))
            {
                var files = Directory.EnumerateFiles(productDir);
                foreach (var file in files)
                {
                    if (Path.GetExtension(file).ToLower() != ".xml") continue;
                    try
                    {
                        var groups = await ConfigReader.LoadXMLConfig<GroupTemp>(file).ConfigureAwait(false);
                        _groups.AddRange(groups.ProductGroups);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="productKey"></param>
        /// <returns></returns>
        public Product GetProduct(string groupName, string productKey)
        {
            var group = _groups.Where(v => v.Name == groupName).FirstOrDefault();
            if (group != null)
            {
                var product = group.Products.Where(v => v.Key == productKey).FirstOrDefault();
                return product;
            }

            return null;
        }

        /// <summary>
        /// 获取第一个产品
        /// </summary>
        /// <param name="productKey"></param>
        /// <returns></returns>
        public Product GetProductFirst(string productKey)
        {
            foreach (var group in _groups)
            {
                var ps = group.Products.Where(v => v.Key == productKey).FirstOrDefault();
                if(ps != null)
                {
                    return ps;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取匹配Key的所有数据
        /// </summary>
        /// <param name="productKey"></param>
        /// <returns></returns>
        public List<Product> GetProductsByKey(string productKey)
        {
            List<Product> products = new List<Product>();
            foreach (var group in _groups)
            {
                var ps = group.Products.Where(v => v.Key == productKey);
                products.AddRange(ps);
            }

            return products;
        }

        class GroupTemp
        {
            public List<ProductGroup> ProductGroups { get; set; }
        }
    }

}
