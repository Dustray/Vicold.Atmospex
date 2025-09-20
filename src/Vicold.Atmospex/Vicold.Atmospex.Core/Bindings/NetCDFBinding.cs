using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vicold.Atmospex.Configration;
using Vicold.Atmospex.Data;
using Vicold.Atmospex.Data.Provider.NetCDF;
using Vicold.Atmospex.FileSystem;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Style;

namespace Vicold.Atmospex.Core.Bindings;
internal class NetCDFBinding : IBinding
{
    private readonly IConfigModuleService? _config;
    private readonly IDataModuleService? _data;
    public NetCDFBinding()
    {
        _config = CoreModuleService.GetService<IConfigModuleService>() ?? throw new Exception("Service not found: IConfigModuleService");
        _data = CoreModuleService.GetService<IDataModuleService>() ?? throw new Exception("Service not found: IDataModuleService");
    }

    public FileHost TryGetFileHost(string filePath)
    {
        if (!NetCDFFileHelper.IsNetCDFFile(filePath)) return null;
        FileCatalog fileCatalog = new NetCDFFileCatalog(filePath);
        var fileHost = new FileHost(filePath, fileCatalog);
        return fileHost;
    }

    public ILayer? CreateLayer(FileHost fileHost)
    {
        if (fileHost is null || _config is null)
        {
            return null;
        }

        var provider = new NetCDFProvider(fileHost);
        provider.LoadData();
        provider.RefreshName();

        var product = provider.Product;
        if (CoreModuleService.Current is { } && CoreModuleService.Current.BindingGridLayer is { })
        {
            var layer = CoreModuleService.Current.BindingGridLayer.Invoke(provider);
            //layer.ID = "123";
            //var name = StylePickUtility.GetPaletteStyleName(provider.ShortName);
            if (product != null)
            {
                layer.Style = StyleHelper.GetPaletteStyle(_config, product.Style);
            }
            else
            {
                layer.Style = StyleHelper.GetDefaultPaletteStyle(_config);
            }

            return layer;
        }

        return null;
    }

}
