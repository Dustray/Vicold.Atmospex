using System;
using System.Collections.Generic;
using System.Text;

namespace Vicold.Atmospex.FileSystem
{
    /// <summary>
    /// 文件托管主机
    /// </summary>
    public sealed class FileHost
    {
        private const string _drive_file_head = @"\\";
        private const string _http_file_head = "http://";

        /// <summary>
        /// 文件列表是否已加载
        /// </summary>
        private bool _isFileListLoaded = false;

        public FileHost(string path) : this(path, FileCatalog.CreateDefaultCatalog(path))
        { }

        public FileHost(string path, FileCatalog catalog)
        {
            Source = Init(path);
            Catalog = catalog;
        }

        public FileCatalog Catalog { get; set; }

        public string Source { get; private set; }

        public Action OnSourceUpdated { get; set; }


        public FileSourceType SourceType { get; private set; }

        public void UpdateSource(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("path is Null");
            }

            Source = Init(path);
            OnSourceUpdated?.Invoke();
        }

        /// <summary>
        /// 数据路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string Init(string path)
        {
            if (path.Length < 3) return path;
            if (path[1] == ':')
            {
                SourceType = FileSourceType.Local;
                path = path.Replace('\\', '/');
            }
            else if (path.StartsWith(_http_file_head))
            {
                SourceType = FileSourceType.Http;
                path = path.Replace('\\', '/');
            }
            else if (path.StartsWith(_drive_file_head))
            {
                SourceType = FileSourceType.Drive;
            }
            else
            {
                SourceType = FileSourceType.Unknown;
                path = path.Replace('\\', '/');
            }

            return path;
        }



    }
}
