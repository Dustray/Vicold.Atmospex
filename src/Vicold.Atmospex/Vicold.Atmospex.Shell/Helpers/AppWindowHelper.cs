using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Vicold.Atmospex.Shell.Contracts.Services;

namespace Vicold.Atmospex.Shell.Helpers;
internal class AppWindowHelper
{

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetDpiForWindow(IntPtr hwnd);

    private static int _openedDPI = 96;
    public static async Task LoadWindowPlacementAsync(WindowEx MainWindow)
    {
        try
        {
            // 在后台线程读取设置，避免阻塞UI线程
            var windowSettings = await Task.Run(async () =>
            {
                var settingsService = App.GetService<ILocalSettingsService>();

                // 使用ConfigureAwait(false)避免捕获上下文
                var windowLeft = await settingsService.ReadSettingAsync<double>("WindowLeft").ConfigureAwait(true);
                var windowTop = await settingsService.ReadSettingAsync<double>("WindowTop").ConfigureAwait(true);
                var windowWidth = await settingsService.ReadSettingAsync<double>("WindowWidth").ConfigureAwait(true);
                var windowHeight = await settingsService.ReadSettingAsync<double>("WindowHeight").ConfigureAwait(true);

                return new { Left = windowLeft, Top = windowTop, Width = windowWidth, Height = windowHeight };
            });

            // 检查是否有有效的窗口位置和大小数据
            if (!double.IsNaN(windowSettings.Left) && !double.IsNaN(windowSettings.Top) &&
                windowSettings.Width > 0 && windowSettings.Height > 0)
            {
                _openedDPI = GetDpiForWindow(MainWindow.GetWindowHandle());
                var physicalLeft = windowSettings.Left;
                var physicalTop = windowSettings.Top;
                var physicalWidth = windowSettings.Width;
                var physicalHeight = windowSettings.Height;

                // 确保窗口在屏幕内
                var displayArea = DisplayArea.GetFromWindowId(MainWindow.AppWindow.Id, DisplayAreaFallback.Nearest);
                if (displayArea != null)
                {
                    // 计算屏幕工作区域的右侧和底部坐标
                    double workAreaRight = displayArea.WorkArea.X + displayArea.WorkArea.Width;
                    double workAreaBottom = displayArea.WorkArea.Y + displayArea.WorkArea.Height;

                    // 确保窗口在屏幕内
                    var clampedLeft = Math.Max(displayArea.WorkArea.X, Math.Min(physicalLeft, workAreaRight - physicalWidth));
                    var clampedTop = Math.Max(displayArea.WorkArea.Y, Math.Min(physicalTop, workAreaBottom - physicalHeight));

                    MainWindow.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(
                        (int)physicalLeft,
                        (int)physicalTop,
                        (int)physicalWidth,
                        (int)physicalHeight));
                }
            }
        }
        catch (Exception ex)
        {
            // 如果加载失败，使用默认设置
            System.Diagnostics.Debug.WriteLine("Failed to load window placement: " + ex.Message);
        }
    }

    public static async void SaveWindowPlacementAsync(WindowEx MainWindow)
    {
        try
        {
            // 获取窗口位置和大小（在UI线程上）
            var rect = MainWindow.AppWindow.Position;
            var size = MainWindow.AppWindow.Size;

            var dpi = GetDpiForWindow(MainWindow.GetWindowHandle());

            // 将物理像素转换为逻辑像素（考虑DPI缩放）
            double logicalLeft = rect.X;
            double logicalTop = rect.Y;
            double logicalWidth = size.Width * _openedDPI / dpi;
            double logicalHeight = size.Height * _openedDPI / dpi;

            // 在后台线程保存设置，使用Fire and Forget模式
            // 这样窗口关闭过程不会被阻塞
            _ = Task.Run(async () =>
            {
                try
                {
                    var settingsService = App.GetService<ILocalSettingsService>();

                    // 使用ConfigureAwait(false)避免捕获上下文
                    await settingsService.SaveSettingAsync("WindowLeft", logicalLeft);
                    await settingsService.SaveSettingAsync("WindowTop", logicalTop);
                    await settingsService.SaveSettingAsync("WindowWidth", logicalWidth);
                    await settingsService.SaveSettingAsync("WindowHeight", logicalHeight);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to save window placement: " + ex.Message);
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Failed to initiate save window placement: " + ex.Message);
        }
    }
}
