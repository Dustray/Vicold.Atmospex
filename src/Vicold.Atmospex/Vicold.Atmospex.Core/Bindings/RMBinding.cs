using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Configration;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Providers;
using Vicold.Atmospex.FileSystem;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Core.Bindings;
internal class RMBinding : IBinding
{
    private readonly IConfigModuleService? _config;
    private readonly IDataModuleService? _data;
    public RMBinding()
    {
        _config = CoreModuleService.GetService<IConfigModuleService>() ?? throw new Exception("Service not found: IConfigModuleService");
        _data = CoreModuleService.GetService<IDataModuleService>() ?? throw new Exception("Service not found: IDataModuleService");
    }

    public FileHost? TryGetFileHost(string filePath)
    {
        var fileEx = Path.GetExtension(filePath);
        if (!fileEx.StartsWith(".rm")) return null;
        FileCatalog fileCatalog = FileCatalog.CreateDefaultCatalog(filePath);
        var fileHost = new FileHost(filePath, fileCatalog);
        return fileHost;
    }

    public ILayer? CreateLayer(FileHost fileHost)
    {
        if (fileHost is null || _config is null || _data is null)
        {
            return null;
        }

        var fileEx = Path.GetExtension(fileHost.Source);
        var fileNa = Path.GetFileNameWithoutExtension(fileHost.Source);
        DataType dataType;
        if (fileEx == ".rm2")
        {
            dataType = DataType.Float32;
        }
        else if (fileEx == ".rm3")
        {
            dataType = DataType.Int64;
        }
        else
        {
            return null;
        }
        var fileArr = fileNa.Split('_');
        var key = "";
        var num = "";
        if (fileArr.Length > 0)
        {
            key = fileArr[0];
            num = fileArr[fileArr.Length - 1];
        }
        var provider = new RMGridProvider(fileHost, dataType);
        provider.LoadData();

        var product = _data.Productor.GetProduct("global", key);
        if (CoreModuleService.Current is { } && CoreModuleService.Current.BindingGridLayer is { })
        {

            var layer = CoreModuleService.Current.BindingGridLayer.Invoke(provider);
            //layer.ID = "123";
            //var name = StylePickUtility.GetPaletteStyleName(provider.ShortName);
            if (product != null)
            {
                layer.Name = $"{product.Display} {num}M";
                layer.Style = StyleHelper.GetPaletteStyle(_config, product.Style);
            }
            else
            {
                layer.Name = fileNa;
                layer.Style = StyleHelper.GetDefaultPaletteStyle(_config);
            }

            return layer;
        }
        return null;
    }

}