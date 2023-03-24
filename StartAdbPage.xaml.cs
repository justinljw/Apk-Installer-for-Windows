using ApkInstallerForWindows.ViewModel;

namespace ApkInstallerForWindows;

public partial class StartAdbPage : ContentPage
{
    private readonly StartAdbViewModel _vm;
    public StartAdbPage(StartAdbViewModel vm)
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