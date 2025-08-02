using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vicold.Atmospex.FileSystem
{
    /// <summary>
    /// 默认文件目录，仅文件当前目录
    /// </summary>
    internal class DefaultFileCatalog : FileCatalog
    {
        private FileInfo[] _fileList;
        private int _fileIndex = -1;
        private bool _loaded = false;
        private string _currentFilePath;

        public DefaultFileCatalog(string filePath, bool lazyLoad)
        {
            _currentFilePath = filePath;
            if (!lazyLoad)
            {
                TryLoad();
            }
        }

        private void Load(string filePath)
        {
            CurrentFolder = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var extension = Path.GetExtension(filePath);
            var folder = new DirectoryInfo(CurrentFolder);
            if (folder.Exists)
            {
                var fileList = folder.GetFiles($"*{extension}", SearchOption.TopDirectoryOnly);
                if (fileList.Length != 0)
                {
                    _fileList = fileList.OrderBy(v => v.Name).ToArray();
                    _fileIndex = _fileList
                        .Select((n, i) => new { Value = n, Index = i })
                        .Aggregate((a, b) => 0 == string.Compare(a.Value.FullName, filePath, true) ? a : b)
                        .Index;
                }
            }
        }

        public override int GetLastDataIndex() => -1;

        public override int GetNextDataIndex() => -1;

        public override string GetLastFile()
        {
            TryLoad();

            if (_fileList == null || _fileIndex == -1)
            {
                return null;
            }

            if (_fileIndex == 0)
            {
                return null;
            }

            _currentFilePath = _fileList[--_fileIndex].FullName;
            return _currentFilePath;
        }


        public override string GetNextFile()
        {
            TryLoad();
            if (_fileList == null || _fileIndex == -1)
            {
                return null;
            }

            if (_fileIndex == _fileList.Length - 1)
            {
                return null;
            }

            _currentFilePath = _fileList[++_fileIndex].FullName;
            return _currentFilePath;
        }

        private void TryLoad()
        {
            if (!_loaded)
            {
                Load(_currentFilePath);
                _loaded = true;
            }
        }

    }
}
