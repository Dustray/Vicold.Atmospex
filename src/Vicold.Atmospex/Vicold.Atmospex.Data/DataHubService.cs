using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.FileSystem;
using System;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Data;
public class DataHubService : IDataHubService
{
    private readonly IAppService _appService;

    public DataHubService(IAppService appService)
    {
        _appService = appService;
    }

    public void DropFiles(string[] filesPath)
    {
        var coreModule = _appService.GetService<object>();
        if (coreModule != null)
        {
            var addDataMethod = coreModule.GetType().GetMethod("AddDataAsync");
            if (addDataMethod != null)
            {
                foreach (var path in filesPath)
                {
                    addDataMethod.Invoke(coreModule, new object[] { path });
                }
            }
        }
    }

    public Task LayerFlipAsync(string layerID, bool isRight)
    {
        return Task.Run(() =>
        {
            var layerModule = _appService.GetService<object>();
            if (layerModule != null)
            {
                var layerManagerProperty = layerModule.GetType().GetProperty("LayerManager");
                if (layerManagerProperty != null)
                {
                    var layerManager = layerManagerProperty.GetValue(layerModule);
                    if (layerManager != null)
                    {
                        var tryGetLayerMethod = layerManager.GetType().GetMethod("TryGetLayer");
                        if (tryGetLayerMethod != null)
                        {
                            var parameters = new object[] { layerID, null };
                            var result = (bool)tryGetLayerMethod.Invoke(layerManager, parameters);
                            if (result)
                            {
                                var layer = parameters[1];
                                var isSuccess = LayerFlip(layer, isRight);
                                if (isSuccess)
                                {
                                    var updateLayerMethod = layerManager.GetType().GetMethod("UpdateLayer");
                                    if (updateLayerMethod != null)
                                    {
                                        updateLayerMethod.Invoke(layerManager, new object[] { layer });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    /// <summary>
    /// 图层翻页
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="isRight"></param>
    private bool LayerFlip(object layer, bool isRight)
    {
        var dataProviderProperty = layer.GetType().GetProperty("DataProvider");
        if (dataProviderProperty != null)
        {
            var provider = dataProviderProperty.GetValue(layer);
            if (provider != null && provider is IFileBoot fileBoot)
            {
                var host = fileBoot.FileHost;
                if (host != null)
                {
                    // 判断左右翻页
                    return Flip(host, provider, isRight);
                }
            }
        }

        return false;
    }

    private bool Flip(FileHost host, object provider, bool isRight)
    {
        // 1. 获取上/下个数据索引
        int lastIndex;
        if (!isRight)
        {
            lastIndex = host.Catalog.GetLastDataIndex(); // 左
        }
        else
        {
            lastIndex = host.Catalog.GetNextDataIndex(); // 右
        }

        // 2. 若为自定义索引，直接跳转自定义索引，否则默认

        if (lastIndex != -1)
        {
            var indexProperty = provider.GetType().GetProperty("Index");
            if (indexProperty != null)
            {
                indexProperty.SetValue(provider, lastIndex);
                return true;
            }
        }

        // 3. 若不是自定义索引到超出边界就翻文件
        var indexProperty2 = provider.GetType().GetProperty("Index");
        var countProperty = provider.GetType().GetProperty("Count");
        if (indexProperty2 != null && countProperty != null)
        {
            int index = (int)indexProperty2.GetValue(provider);
            int count = (int)countProperty.GetValue(provider);
            index += isRight ? 1 : -1;
            if (index < 0 || index >= count)
            {
                return Update();
            }
            indexProperty2.SetValue(provider, index);
            return true;
        }

        return true;

        bool Update()
        {
            // 到头，找上一个文件
            string newFile;

            if (!isRight)
            {
                newFile = host.Catalog.GetLastFile(); // 左
            }
            else
            {
                newFile = host.Catalog.GetNextFile(); // 右
            }

            if (newFile != null)
            {
                host.UpdateSource(newFile);
                var reloadDataMethod = provider.GetType().GetMethod("ReLoadData");
                if (reloadDataMethod != null)
                {
                    reloadDataMethod.Invoke(provider, null);
                }
                var indexProperty3 = provider.GetType().GetProperty("Index");
                var countProperty3 = provider.GetType().GetProperty("Count");
                if (indexProperty3 != null && countProperty3 != null)
                {
                    if (!isRight)
                    {
                        int count = (int)countProperty3.GetValue(provider);
                        indexProperty3.SetValue(provider, count - 1);
                    }
                    else
                    {
                        indexProperty3.SetValue(provider, 0);
                    }
                }
            }
            else
            {
                //到头了
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 命令执行
    /// </summary>
    /// <param name="layerID"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public Task OrderExcuteAsync(string layerID, string order)
    {
        return Task.Run(() =>
        {
            var layerModule = _appService.GetService<object>();
            if (layerModule != null)
            {
                var layerManagerProperty = layerModule.GetType().GetProperty("LayerManager");
                if (layerManagerProperty != null)
                {
                    var layerManager = layerManagerProperty.GetValue(layerModule);
                    if (layerManager != null)
                    {
                        var tryGetLayerMethod = layerManager.GetType().GetMethod("TryGetLayer");
                        if (tryGetLayerMethod != null)
                        {
                            var parameters = new object[] { layerID, null };
                            var result = (bool)tryGetLayerMethod.Invoke(layerManager, parameters);
                            if (result)
                            {
                                var layer = parameters[1];
                                switch (order)
                                {
                                    case "smooth":
                                        var dataProviderProperty = layer.GetType().GetProperty("DataProvider");
                                        if (dataProviderProperty != null)
                                        {
                                            var provider = dataProviderProperty.GetValue(layer);
                                            if (provider != null)
                                            {
                                                var smoothRollMethod = provider.GetType().GetMethod("SmoothRoll");
                                                if (smoothRollMethod != null)
                                                {
                                                    smoothRollMethod.Invoke(provider, null);
                                                }
                                            }
                                        }

                                        var updateLayerMethod = layerManager.GetType().GetMethod("UpdateLayer");
                                        if (updateLayerMethod != null)
                                        {
                                            updateLayerMethod.Invoke(layerManager, new object[] { layer });
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        });
    }
}
