using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Evergine.Common.Graphics;
using Evergine.DirectX11;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.WinUI;
using Windows.Storage;
using Microsoft.UI;
using Vicold.Atmospex.Render.Serviecs;
using Vicold.Atmospex.Earth;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Vicold.Atmospex.Render.Frame.Views
{
    public sealed partial class RenderView : UserControl
    {
        private MouseInteractionService? interactionService;
        public nint Hwnd
        {
            get; set;
        }

        private int _frameCount = 0;
        private DateTime _lastUpdateTime;
        private const double UpdateIntervalMilliseconds = 1000; // 每秒更新一次

        public RenderView()
        {
            this.InitializeComponent();
            SwapChainPanel.Loaded += OnSwapChainPanelLoaded;
            _lastUpdateTime = DateTime.Now;
        }

        private void OnSwapChainPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (interactionService is { }) return;
            // Create app
            MyApplication application = new MyApplication();

            //interactionService = new InteractionService();
            //application.Container.RegisterInstance(interactionService);

            interactionService = new MouseInteractionService();
            application.Container.RegisterInstance(interactionService);

            GraphicsContext graphicsContext = new DX11GraphicsContext();
            application.Container.RegisterInstance(graphicsContext);
            graphicsContext.CreateDevice();

            // Create Services
            WinUIWindowsSystem windowsSystem = new WinUIWindowsSystem();
            application.Container.RegisterInstance(windowsSystem);

            var surface = (WinUISurface)windowsSystem.CreateSurface(SwapChainPanel);

            ConfigureGraphicsContext(application, surface, "DefaultDisplay");

            // Creates XAudio device
            var xaudio = new Evergine.XAudio2.XAudioDevice();
            application.Container.RegisterInstance(xaudio);

            Stopwatch clockTimer = Stopwatch.StartNew();
            windowsSystem.Run(
            () =>
            {
                application.Initialize();
            },
            () =>
            {
                var gameTime = clockTimer.Elapsed;
                clockTimer.Restart();

                application.UpdateFrame(gameTime);
                application.DrawFrame(gameTime);
                UpdateFPS();
            });

            // 服务设置

            var earthModuleService = RenderModuleService.GetService<IEarthModuleService>();


            RenderModuleService.Current?.BindEntityManager(application.Scene?.Managers.EntityManager);
            RenderModuleService.Current?.BindCurrentMouseInteractionService(interactionService);
            RenderModuleService.GetService<Vicold.Atmospex.Core.ICoreModuleService>()?.OnViewStart?.Invoke();

            interactionService.MouseMoveChangedEvent += (s, args) =>
            {
                earthModuleService?.ChangeMouse(args.WorldPosition.X, args.WorldPosition.Y, args.ScreenPosition.X, args.ScreenPosition.Y);
            };
        }

        private void UpdateFPS()
        {
            // 计算帧率
            _frameCount++;
            var currentTime = DateTime.Now;
            var elapsedTime = currentTime - _lastUpdateTime;

            if (elapsedTime.TotalMilliseconds >= UpdateIntervalMilliseconds)
            {
                // 计算帧率
                double fps = _frameCount / elapsedTime.TotalSeconds;
                int roundedFps = (int)Math.Round(fps);

                // 通过CoreModuleService更新FPS
                var coreModuleService = RenderModuleService.GetService<Vicold.Atmospex.Core.ICoreModuleService>();
                coreModuleService?.SetFps(roundedFps);

                // 重置计数器
                _frameCount = 0;
                _lastUpdateTime = currentTime;
            }
        }

        private static void ConfigureGraphicsContext(Evergine.Framework.Application application, WinUISurface surface, string displayName)
        {
            GraphicsContext graphicsContext = application.Container.Resolve<GraphicsContext>();

            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = surface.SurfaceInfo,
                Width = surface.Width,
                Height = surface.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60
            };

            var swapChain = graphicsContext.CreateSwapChain(swapChainDescription);
            swapChain.VerticalSync = true;
            surface.NativeSurface.SwapChain = swapChain;

            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            var firstDisplay = new Display(surface, swapChain);
            graphicsPresenter.AddDisplay(displayName, firstDisplay);
        }

        private void OnSwapChainPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ((SwapChainPanel)sender).Focus(FocusState.Pointer);
            ((SwapChainPanel)sender).CapturePointer(e.Pointer);
        }

        private void OnSwapChainPanelPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ((SwapChainPanel)sender).ReleasePointerCaptures();
        }

        private void ResetCamera_Click(object sender, RoutedEventArgs e)
        {
            interactionService?.ResetRotation();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
