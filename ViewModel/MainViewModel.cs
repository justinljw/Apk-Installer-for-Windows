using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.Json;

namespace ApkInstallerForWindows.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        ApkFilePath = null;
        JsonPath = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
        CanClick = false;
        AppName = "";
        ReleaseId = "latest";
        AppcenterHint = "Survey123-dotnet_arm64_apk\nSurvey123Alpha_arm64.apk\nArcGISQuickCapture_arm64.apk\n\n\n";
    }

    [ObservableProperty]
    public static string apkFilePath;

    [ObservableProperty]
    public static string jsonPath;

    [ObservableProperty]
    private bool canClick;

    [ObservableProperty]
    public static string appName;

    [ObservableProperty]
    public static string releaseId;

    [ObservableProperty]
    public static Info userInfo;

    [ObservableProperty]
    public static bool canInstall;

    [ObservableProperty]
    private string appcenterHint;

    [RelayCommand]
    async void Select()
    {
        var apkfile = readFile();
        await apkfile;
        if (apkfile.Result != null)
        {
            ApkFilePath = apkfile.Result;
            CanClick = true;
        }
    }

    [RelayCommand]
    async Task Next()
    {
        await Shell.Current.GoToAsync(nameof(LaunchWsaPage));
    }

    [RelayCommand]
    public async Task ProceedTo()
    {
        await Shell.Current.GoToAsync(nameof(ApkInstallPage));
    }

    [RelayCommand]
    async Task Open()
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    [RelayCommand]
    async void Install()
    {
        await Shell.Current.GoToAsync(nameof(AppcenterDownloadPage));
    }

    private async Task<String> readFile()
    {
        try
        {
            var apkFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".apk" } },
                });

            var results = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Please select an Android apk file",
                FileTypes = apkFileType,
            });
            return results == null ? throw new Exception() : results.FullPath;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("No file selected", $"Cancelled. {ex.Message}", "OK");
        }
        return null;
    }

    public Task startReadJsonTask()
    {
        return Task.Factory.StartNew(() => readJson());
    }

    private async Task readJson()
    {
        try
        {
            if (!File.Exists(JsonPath))
            {
                UserInfo = new Info() { thisWsaPort = "58526", thisTokenStr = "", thisOwnerName = "appstudio-h0at", thisDownloadPath = "default", thisAutoLaunch = false };
                writeJson(UserInfo, JsonPath);
            }
            using (StreamReader strRead = File.OpenText(JsonPath))
            {
                string dataText = await strRead.ReadToEndAsync();
                UserInfo = JsonSerializer.Deserialize<Info>(dataText);
            }
        }
        catch (FileNotFoundException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Cannot read settings", 
                $"May be lack of settings.json file. Settings will not be saved. {ex.Message}", 
                "OK");
        }
    }

    private void writeJson(Info thisData, string thisPath)
    {
        try
        {
            using (StreamWriter strWrite = File.CreateText(thisPath))
            {
                string strJson = JsonSerializer.Serialize(thisData);
                strWrite.WriteLine(strJson);
            }
        }
        catch (FileNotFoundException ex)
        {
            Application.Current.MainPage.DisplayAlert("Cannot save settings",
                $"May be lack of settings.json file. Settings will not be saved. {ex.Message}",
                "OK");
        }
    }

    public Task startCanInstallTask()
    {
        return Task.Factory.StartNew(() => canInstallButton());
    }

    private void canInstallButton()
    {
        CanInstall = (
            UserInfo != null &&
            userInfo.thisWsaPort != null &&
            userInfo.thisTokenStr != null &&
            userInfo.thisOwnerName != null &&
            userInfo.thisDownloadPath != null) &&
            (userInfo.thisWsaPort != "" && 
            userInfo.thisTokenStr != "" && 
            userInfo.thisOwnerName != "" &&
            userInfo.thisDownloadPath != "")
            ? true : false;
    }
}
