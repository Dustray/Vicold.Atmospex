using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Effects;
using Evergine.Framework.Managers;
using Evergine.Framework.Services;
using System;
using Vicold.Atmospex.CoreService;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Render.Frame.Layers;

namespace Vicold.Atmospex.Render.Frame;

public class RenderModuleService : IRenderModuleService
{
    private static IAppService? _appService;
    private EntityManager? _entityManager;
    public static RenderModuleService? Current
    {
        get; private set;
    }

    public RenderModuleService(IAppService appService)
    {
        _appService = appService;
        Current = this;
    }

    public void Initialize()
    {
    }

    internal static T? GetService<T>() where T : class
    {
        return _appService?.GetService<T>();
    }

    public void Bind(ILayerModuleService layerModuleService)
    {
        layerModuleService.LayerManager.OnLayerAdded += (s, e) =>
        {
            if (e.Layer is IRenderLayer el&& _entityManager is { })
            {
                el.Draw(_entityManager);
             
            }
        };
    }

    internal void BindEntityManager(EntityManager? entityManager)
    {
        _entityManager = entityManager;
    }
}