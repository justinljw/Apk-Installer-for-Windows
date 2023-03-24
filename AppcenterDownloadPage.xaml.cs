using ApkInstallerForWindows.ViewModel;

namespace ApkInstallerForWindows;

public partial class AppcenterDownloadPage : ContentPage
{
    private readonly AppcenterDownloadViewModel _vm;
    public AppcenterDownloadPage(AppcenterDownloadViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        LoadAfterConstruction();
    }

    private async void LoadAfterConstruction()
    {
        await _vm.startTask();
    }
}