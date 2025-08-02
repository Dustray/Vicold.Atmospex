using Vicold.Atmospex.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vicold.Atmospex.Data.Provider.NetCDF
{
    public class NetCDFFileCatalog : FileCatalog
    {
        private FileInfo[] _fileList;
        private int _fileIndex = -1;
        private string _fileExtension;
        private bool _loaded = false;
        private string _currentFilePath;

        public NetCDFFileCatalog(string filePath, bool lazyLoad = true)
        {
            _currentFilePath = filePath;
            if (!lazyLoad)
            {
                TryLoad();
            }
            FolderFormat = GetPathFormat(filePath);
        }


        public string FolderFormat { get; private set; }

        public DateTime CurrentDateTime { get; set; }

        #region FileCatalog

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
                // 尝试翻到上一页
                var lastFolder = GetLastFolder();
                if (lastFolder == null) return null; // 没有上一个文件夹

                var fileList = GetFileInfos(lastFolder);
                if (fileList == null) return null; // 有上一个文件夹但是没有数据

                CurrentFolder = lastFolder;
                _fileList = fileList.OrderBy(v => v.Name).ToArray();
                _fileIndex = _fileList.Length - 1;
                return _fileList[_fileIndex].FullName;
            }
            else
            {
                // 还在本文件夹，继续翻
                return _fileList[--_fileIndex].FullName;
            }
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
                // 尝试翻到下一页
                var nextFolder = GetNextFolder();
                if (nextFolder == null) return null; // 没有下一个文件夹

                var fileList = GetFileInfos(nextFolder);
                if (fileList == null) return null; // 有下一个文件夹但是没有数据

                CurrentFolder = nextFolder;
                _fileList = fileList.OrderBy(v => v.Name).ToArray();
                _fileIndex = 0;
                return _fileList[_fileIndex].FullName;
            }
            else
            {
                // 还在本文件夹，继续翻
                return _fileList[++_fileIndex].FullName;
            }
        }

        #endregion

        #region GetFolder

        private string GetLastFolder()
        {
            // Godot.GD.Print(CurrentDateTime);
            var dateTime = CurrentDateTime.AddYears(-1);
            return GetFolder(dateTime);
        }

        private string GetNextFolder()
        {
            var dateTime = CurrentDateTime.AddYears(1);
            return GetFolder(dateTime);
        }


        private string GetFolder(DateTime dateTime)
        {
            if (FolderFormat == null)
            {
                return CurrentFolder;
            }

            var folder = string.Format(FolderFormat, dateTime.Year);
            if (Directory.Exists(folder))
            {
                CurrentDateTime = dateTime;
                CurrentFolder = folder;
                return folder;
            }

            return null;
        }

        #endregion

        private FileInfo[] GetFileInfos(string folderPath)
        {
            var folder = new DirectoryInfo(folderPath);
            if (folder.Exists)
            {
                return folder.GetFiles($"*{_fileExtension}", SearchOption.TopDirectoryOnly);
            }

            return null;
        }

        private void TryLoad()
        {
            if (!_loaded)
            {
                Load(_currentFilePath);
                _loaded = true;
            }
        }

        private void Load(string filePath)
        {
            CurrentFolder = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            InitCurrentDateTime(fileNameWithoutExtension);
            var extension = Path.GetExtension(filePath);
            _fileExtension = extension;
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

        private void InitCurrentDateTime(string dateStr)
        {
            if (dateStr.Length != 8 || !int.TryParse(dateStr, out var date))
            {
                return;
            }
            CurrentDateTime = DateTime.ParseExact(dateStr, "yyyyMMdd", null);
        }

        private string GetPathFormat(string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            folderPath = folderPath.Replace('\\', '/');
            var pathSplit = folderPath.Split('/');

            var time = pathSplit[pathSplit.Length - 1];

            if (time.Length != 4 || !int.TryParse(time, out var year))
            {
                return null;
            }

            pathSplit[pathSplit.Length - 1] = "{0}";
            return string.Join("/", pathSplit);
        }

    }
}
