namespace ApkInstallerForWindows;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(ApkInstallPage), typeof(ApkInstallPage));
        Routing.RegisterRoute(nameof(LaunchWsaPage), typeof(LaunchWsaPage));
        Routing.RegisterRoute(nameof(StartAdbPage), typeof(StartAdbPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(AppcenterDownloadPage), typeof(AppcenterDownloadPage));
    }
}
