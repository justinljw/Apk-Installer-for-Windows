using ApkInstallerForWindows.ViewModel;
using System.Diagnostics;

namespace ApkInstallerForWindows;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    public MainPage(MainViewModel vm)
    {
        BindingContext = _vm = vm;
        LoadBeforeConstruction();
        InitializeComponent();
        
        string[] arguments = Environment.GetCommandLineArgs();
        if (App.argskind == "File")
        {
            MainViewModel.apkFilePath = arguments[1];
            LoadAfterConstruction();
        }
    }

    private async void LoadAfterConstruction()
    {
        await _vm.ProceedTo();
    }

    private async void LoadBeforeConstruction()
    {
        await _vm.startReadJsonTask();
        await _vm.startCanInstallTask();
    }

    void OnReleaseEtyTextChanged(object sender, TextChangedEventArgs e)
    {
        _vm.ReleaseId = ReleaseEty.Text;
    }

    void OnAppEtyTextChanged(object sender, TextChangedEventArgs e)
    {
        _vm.AppName = AppEty.Text;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.startCanInstallTask();
    }
}

