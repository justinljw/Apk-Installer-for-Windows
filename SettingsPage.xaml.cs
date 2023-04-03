using ApkInstallerForWindows.ViewModel;

namespace ApkInstallerForWindows;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;

    public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
        BindingContext = _vm = vm;
    }

    void OnWsaEtyTextChanged(object sender, TextChangedEventArgs e)
    {
        MainViewModel.userInfo.thisWsaPort = WsaEty.Text;
    }

    void OnTokenEtyTextChanged(object sender, TextChangedEventArgs e)
    {
        MainViewModel.userInfo.thisTokenStr = TokenEty.Text;
    }

    void OnOwnerEtyTextChanged(object sender, TextChangedEventArgs e)
    {
        MainViewModel.userInfo.thisOwnerName = OwnerEty.Text;
    }

    void OnDownloadEtyTextChanged(object sender, TextChangedEventArgs e)
    {
        MainViewModel.userInfo.thisDownloadPath = DownloadEty.Text;
    }

    void OnAutoLaunchChbCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        MainViewModel.userInfo.thisAutoLaunch = e.Value;
    }
}