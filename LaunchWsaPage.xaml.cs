using ApkInstallerForWindows.ViewModel;

namespace ApkInstallerForWindows;

public partial class LaunchWsaPage : ContentPage
{
    private readonly LaunchWsaViewModel _vm;
    public LaunchWsaPage(LaunchWsaViewModel vm)
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