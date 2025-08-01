using Microsoft.UI.Xaml.Controls;

using Vicold.Atmospex.Shell.ViewModels;

namespace Vicold.Atmospex.Shell.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
    }
}
