using Newtonsoft.Json;
using Vicold.Atmospex.CoreService;

namespace Vicold.Atmospex.Configration;

public class ConfigModuleService : IConfigModuleService
{
    private static IAppService? _appService;

    public static ConfigModuleService? Current
    {
        get; private set;
    }


    public ConfigModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
    }




    public string WorkSpace
    {
        get; private set;
    } = string.Empty;

    public string StyleHome
    {
        get; private set;
    } = string.Empty;

    public string PaletteHome
    {
        get; private set;
    } = string.Empty;

    private static BootConfig GetOrCreateBootConfig()
    {

        var jsonPath = "./Config/boot.json";
            BootConfig? bootConfig;
        if (System.IO.File.Exists(jsonPath))
        {
            var bootJson = System.IO.File.ReadAllText(jsonPath);
            try
            {
                bootConfig = JsonConvert.DeserializeObject<BootConfig>(bootJson);
                if(bootConfig == null)
                {
                    throw new Exception("Boot configuration wrong.");
                }

                return bootConfig;
            }
            catch
            {
                throw new Exception("Boot configuration wrong.");
            }
        }
        else
        {
            bootConfig = new BootConfig()
            {
                WorkSpace = Path.GetFullPath("./"),
                WorkSpaceDebug = Path.GetFullPath("./"),
            };
            JsonConvert.SerializeObject(bootConfig, Formatting.Indented);
            return bootConfig;
        }
    }

    public void Initialize()
    {
        Init(GetOrCreateBootConfig());
    }

    public void Init(BootConfig bootConfig)
    {

#if DEBUG
        WorkSpace = bootConfig.WorkSpaceDebug;
#else
        WorkSpace = bootConfig.WorkSpace;
#endif

        StyleHome = Path.Combine(WorkSpace, "data/style");
        PaletteHome = Path.Combine(StyleHome, "palette");
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }
}