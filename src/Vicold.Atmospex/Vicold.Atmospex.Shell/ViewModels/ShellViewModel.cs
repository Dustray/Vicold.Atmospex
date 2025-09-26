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
using Vicold.Atmospex.Shell.Contracts.Services;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Vicold.Atmospex.Shell.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    private ICoreModuleService _coreModuleService;
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


    public ShellViewModel(INavigationService navigationService, IEarthModuleService earthModuleService, ICoreModuleService coreModuleService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        _coreModuleService = coreModuleService;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuSettingsCommand = new RelayCommand(OnMenuSettings);
        MenuViewsMainCommand = new RelayCommand(OnMenuViewsMain);
        MenuFileOpenCommand = new RelayCommand(OnMenuFileOpen);

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

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

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
