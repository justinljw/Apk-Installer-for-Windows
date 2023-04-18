using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Storage;
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
        AutoLaunch = MainViewModel.userInfo.thisAutoLaunch;
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
    public static bool autoLaunch;

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
        MainViewModel.userInfo.thisAutoLaunch = AutoLaunch;

        try
        {
            startMoveAapt();
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

    private Task startMoveAapt()
    {
        return Task.Factory.StartNew(() => moveAapt());
    }

    private async void moveAapt()
    {
        try
        {
            string sourceFile = "aapt.exe";
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, sourceFile);
            if (!File.Exists(targetFile))
            {
                using FileStream outputStream = File.OpenWrite(targetFile);
                using Stream fs = await FileSystem.Current.OpenAppPackageFileAsync(sourceFile);
                using BinaryWriter writer = new BinaryWriter(outputStream);
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    var bytesRead = 0;

                    int bufferSize = 1024;
                    var buffer = new byte[bufferSize];
                    using (fs)
                    {
                        do
                        {
                            buffer = reader.ReadBytes(bufferSize);
                            bytesRead = buffer.Count();
                            writer.Write(buffer);
                        }

                        while (bytesRead > 0);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AutoLaunch = false;
            MainViewModel.userInfo.thisAutoLaunch = AutoLaunch;
            await Application.Current.MainPage.DisplayAlert("May be lack of aapt.exe", $"Can't use auto launch mode. {ex.Message}", "OK");
        }
    }
}
