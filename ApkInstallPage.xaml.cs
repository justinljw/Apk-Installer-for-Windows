using ApkInstallerForWindows.ViewModel;

namespace ApkInstallerForWindows;

public partial class ApkInstallPage : ContentPage
{
	public ApkInstallPage(ApkInstallViewModel vm)
	{
        string[] arguments = Environment.GetCommandLineArgs();
        ApkInstallViewModel.apkFileName = Path.GetFileName(arguments[1]);

        InitializeComponent();
		BindingContext = vm;
	}
}