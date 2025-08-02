using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.FileSystem
{
    /// <summary>
    /// 文件目录，翻页使用
    /// </summary>
    public abstract class FileCatalog
    {
        public string CurrentFolder { get; set; }

        /// <summary>
        /// 获取下一个数据索引，返回-1则使用默认数据索引
        /// </summary>
        /// <returns></returns>
        public abstract int GetLastDataIndex();

        /// <summary>
        /// 获取下一个数据索引，返回-1则使用默认数据索引
        /// </summary>
        /// <returns></returns>
        public abstract int GetNextDataIndex();

        /// <summary>
        /// 获取下一个文件，返回null则到头
        /// </summary>
        /// <returns></returns>
        public abstract string GetLastFile();

        /// <summary>
        /// 获取下一个文件，返回null则到头
        /// </summary>
        /// <returns></returns>
        public abstract string GetNextFile();



        /// <summary>
        /// 默认目录
        /// </summary>
        /// <returns></returns>
        public static FileCatalog CreateDefaultCatalog(string filePath, bool lazyLoad = true)
        {
            var defaultCatalog = new DefaultFileCatalog(filePath, lazyLoad);

            return defaultCatalog;
        }
    }
}
