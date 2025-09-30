using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Vicold.Atmospex.Core;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.FileSystem;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Layer.Events;
using Vicold.Atmospex.Shell.Contracts.Services;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Vicold.Atmospex.Shell.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    private ICoreModuleService _coreModuleService;
    private ILayerManager _layerManager;
    [ObservableProperty]
    private bool isBackEnabled;

    private int _fps = 0;
    public int Fps
    {
        get => _fps;
        set
        {
            _fps = value;
            OnPropertyChanged();
        }
    }

    // 添加图层集合，使用现有的ILayer接口
    public ObservableCollection<ILayer> Layers { get; private set; } = new ObservableCollection<ILayer>();

    public ICommand MenuFileExitCommand
    {
        get;
    }

    public ICommand MenuSettingsCommand
    {
        get;
    }

    public ICommand MenuViewsMainCommand
    {
        get;
    }

    public ICommand MenuFileOpenCommand
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    private float _longtitude = 0;
    public float Longtitude
    {
        get => _longtitude;
        set
        {
            _longtitude = value;
            OnPropertyChanged(nameof(LongtitudeDisplay));
        }
    }

    private float _latitude = 0;
    public float Latitude
    {
        get => _latitude;
        set
        {
            _latitude = value;
            OnPropertyChanged(nameof(LatitudeDisplay));
        }
    }

    public string LongtitudeDisplay => $"{Math.Abs(Math.Min(180, Math.Max(-180, Longtitude))):F4}° {(Longtitude >= 0 ? "E" : "W")}";
    public string LatitudeDisplay => $"{Math.Abs(Math.Min(90, Math.Max(-90, Latitude))):F4}° {(Latitude >= 0 ? "N" : "S")}";


    public ShellViewModel(INavigationService navigationService, IEarthModuleService earthModuleService, ICoreModuleService coreModuleService, ILayerModuleService layerModuleService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        _coreModuleService = coreModuleService;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuSettingsCommand = new RelayCommand(OnMenuSettings);
        MenuViewsMainCommand = new RelayCommand(OnMenuViewsMain);
        MenuFileOpenCommand = new RelayCommand(OnMenuFileOpen);

        // 从LayerModuleService获取图层管理器
        _layerManager = layerModuleService.LayerManager;

        // 初始化图层集合
        UpdateLayers();

        // 订阅图层变化事件
        _layerManager.OnLayerAdded += OnLayerAdded;
        _layerManager.OnLayerRemoved += OnLayerRemoved;
        _layerManager.OnLayerUpdated += OnLayerUpdated;

        // 订阅FPS变化事件
        coreModuleService.OnFpsChanged += (fps) =>
        {
            Fps = fps;
        };

        earthModuleService.OnMouseMoved += (s, args) =>
        {
            Longtitude = args.GeoCoordinate.X;
            Latitude = args.GeoCoordinate.Y;
        };
    }

    /// <summary>
    /// 更新图层列表
    /// </summary>
    private void UpdateLayers()
    {
        ExecuteOnUiThread(() =>
        {
            Layers.Clear();
            foreach (var layer in _layerManager.GetAllLayers())
            {
                Layers.Add(layer);
            }
        });
    }

    /// <summary>
    /// 图层添加事件处理
    /// </summary>
    private void OnLayerAdded(object sender, LayerChangedEventArgs e)
    {
        if (e.Layer != null)
        {
            ExecuteOnUiThread(() =>
            {
                Layers.Add(e.Layer);
            });
        }
    }

    /// <summary>
    /// 图层移除事件处理
    /// </summary>
    private void OnLayerRemoved(object sender, LayerChangedEventArgs e)
    {
        if (e.Layer != null)
        {
            ExecuteOnUiThread(() =>
            {
                Layers.Remove(e.Layer);
            });
        }
    }

    /// <summary>
    /// 图层更新事件处理
    /// </summary>
    private void OnLayerUpdated(object sender, LayerChangedEventArgs e)
    {
        ExecuteOnUiThread(() =>
        {
            // 触发属性变更通知
            OnPropertyChanged(nameof(Layers));
        });
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    /// <summary>
    /// 在UI线程上执行操作的通用帮助方法
    /// </summary>
    /// <param name="action">要在UI线程上执行的操作</param>
    private void ExecuteOnUiThread(Action action)
    {
        if (action == null)
        {
            return;
        }

        App.MainWindow.DispatcherQueue.TryEnqueue(() => action());
    }

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnMenuViewsMain() => NavigationService.NavigateTo(typeof(MainViewModel).FullName!);

    private async void OnMenuFileOpen()
    {
        // 创建文件选择器
        var filePicker = new FileOpenPicker();

        // 获取当前窗口的HWND，这在WinUI 3中是必需的
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        // 设置文件选择器的窗口句柄
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hWnd);

        // 添加文件类型筛选器
        filePicker.FileTypeFilter.Add(".nc"); // NetCDF文件
        filePicker.FileTypeFilter.Add(".csv"); // CSV文件
        filePicker.FileTypeFilter.Add(".json"); // JSON文件
        filePicker.FileTypeFilter.Add(".xml"); // XML文件
        filePicker.FileTypeFilter.Add("*"); // 所有文件

        // 显示文件选择器并等待用户选择
        var file = await filePicker.PickSingleFileAsync();

        if (file != null)
        {
            System.Diagnostics.Debug.WriteLine("Selected file: " + file.Path);
            await _coreModuleService.AddDataAsync(file.Path);
        }
    }
}
