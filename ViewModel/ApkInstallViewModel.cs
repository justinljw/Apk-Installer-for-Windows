using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ApkInstallerForWindows.ViewModel;

public partial class ApkInstallViewModel : ObservableObject
{
    [ObservableProperty]
    public static string apkFileName;

    [RelayCommand]
    async Task Install()
    {
        await Shell.Current.GoToAsync(nameof(LaunchWsaPage));
    }
}
