using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Storage;
using System.Diagnostics;
using System.Text.Json;

namespace ApkInstallerForWindows.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel()
    {
        WsaPort = MainViewModel.userInfo.thisWsaPort;
        TokenStr = MainViewModel.userInfo.thisTokenStr;
        OwnerName = MainViewModel.userInfo.thisOwnerName;
        DownloadPath = MainViewModel.userInfo.thisDownloadPath;
        ShowSave = false;
        AppVersion = AppInfo.Current.VersionString;
    }

    [ObservableProperty]
    public static string wsaPort;

    [ObservableProperty]
    public static string tokenStr;

    [ObservableProperty]
    public static string ownerName;

    [ObservableProperty]
    public static string downloadPath; 

    [ObservableProperty]
    private bool showSave;

    [ObservableProperty]
    private string appVersion;

    [RelayCommand]
    async void Select()
    {
        DownloadPath = await readFolder();
    }

    private async Task<String> readFolder()
    {
        try
        {
            var folderPath = await FolderPicker.PickAsync(default);


            //Debug.WriteLine(":::::::step0");
            //FolderPicker folderPicker = new FolderPicker();
            //Debug.WriteLine(":::::::step1");
            //folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            //folderPicker.FileTypeFilter.Add("*");
            //Debug.WriteLine(":::::::step2");
            //var folder = await folderPicker.PickSingleFolderAsync();
            //Debug.WriteLine(":::::::step3");
            return folderPath == null ? throw new Exception() : folderPath.Folder.Path;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("No folder selected", $"{ex.Message}", "OK");
        }
        return "default";
    }

    [RelayCommand]
    private void Save()
    {
        MainViewModel.userInfo.thisWsaPort = WsaPort;
        MainViewModel.userInfo.thisTokenStr = TokenStr;
        MainViewModel.userInfo.thisOwnerName = OwnerName;
        MainViewModel.userInfo.thisDownloadPath = DownloadPath;

        try
        {
            using (StreamWriter strWrite = File.CreateText(MainViewModel.jsonPath))
            {
                string strJson = JsonSerializer.Serialize(MainViewModel.userInfo);
                strWrite.WriteLine(strJson);
            }
            ShowSave = true;
        }
        catch (FileNotFoundException ex)
        {
            Application.Current.MainPage.DisplayAlert("Settings not saved!",
                $"May be lack of settings.json file. Settings can not be saved. {ex.Message}",
                "OK");
        }
    }
}
