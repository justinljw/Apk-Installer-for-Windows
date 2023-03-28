using AppCenterDownloadAppGetAppInfo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ApkInstallerForWindows.ViewModel;

public partial class AppcenterDownloadViewModel : ObservableObject
{
    string tempAppLocation = MainViewModel.userInfo.thisDownloadPath != "default"? 
        MainViewModel.userInfo.thisDownloadPath : FileSystem.Current.CacheDirectory;
    public AppcenterDownloadViewModel()
    {
        LaunchStatus = null;
        CanClick = false;
        IsBusy = true;
        showInfo = false;
        savedFilePath = "";
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    bool canClick;

    [ObservableProperty]
    bool showInfo;

    [ObservableProperty]
    string launchStatus;

    [ObservableProperty]
    public static string savedFilePath;

    [RelayCommand]
    async Task Next()
    {
        await Shell.Current.GoToAsync(nameof(LaunchWsaPage));
    }

    public Task startTask()
    {
        return Task.Factory.StartNew(() => start_Download());
    }

    private void start_Download()
    {
        try
        {
            AppCenter appCenter = new AppCenter(MainViewModel.userInfo.thisTokenStr, MainViewModel.userInfo.thisOwnerName);
            string getApp = appCenter.DownloadApplication(MainViewModel.appName, MainViewModel.releaseId, tempAppLocation, false);
            IsBusy = false;
            CanClick = true;
            ShowInfo = true;
            LaunchStatus = "Temporary file saved at:";
            SavedFilePath = getApp;
        }
        catch (Exception ex)
        {
            try
            {
                IsBusy = false;
                CanClick = false;
                ShowInfo = true;
                LaunchStatus = "Download failed!";
                SavedFilePath = $"{ex.Message}";
            }
            catch (Exception exSub)
            {
                IsBusy = false;
                CanClick = false;
                ShowInfo = true;
                LaunchStatus = "Download failed! (Unknown issue)";
                SavedFilePath = $"{exSub.Message}";
            }
        }
    }
}
