using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Configration;

public interface IConfigModuleService : IModuleService
{
    string WorkSpace
    {
        get;
    }

    string StyleHome
    {
        get;
    }

    string PaletteHome
    {
        get;
    }

    void Init(BootConfig bootConfig);
}