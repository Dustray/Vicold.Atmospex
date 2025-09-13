using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Shell.Contracts.Services;

namespace Vicold.Atmospex.Shell.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;

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

    public string LongtitudeDisplay => $"{Math.Abs(Math.Min(180, Math.Max(-180,Longtitude))):F4}° {(Longtitude >= 0 ? "E" : "W")}";
    public string LatitudeDisplay => $"{Math.Abs(Math.Min(90, Math.Max(-90, Latitude))):F4}° {(Latitude >= 0 ? "N" : "S")}";


    public ShellViewModel(INavigationService navigationService, IEarthModuleService earthModuleService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuSettingsCommand = new RelayCommand(OnMenuSettings);
        MenuViewsMainCommand = new RelayCommand(OnMenuViewsMain);


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
}
