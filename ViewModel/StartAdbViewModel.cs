using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ApkInstallerForWindows.ViewModel;

public partial class StartAdbViewModel : ObservableObject
{
    private string installFilePath = MainViewModel.apkFilePath != null && MainViewModel.apkFilePath != ""
        ? MainViewModel.apkFilePath : AppcenterDownloadViewModel.savedFilePath;
    private string appLaunchCmdOut = "";

    public StartAdbViewModel()
    {
        InstallStatus = null;
        CanClick = false;
        IsBusy = true;
        ShowInfo = false;
        IsStartAppLaunch = false;
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    bool canClick;

    [ObservableProperty]
    bool showInfo;

    [ObservableProperty]
    String installStatus;

    [ObservableProperty]
    bool isStartAppLaunch;

    [RelayCommand]
    private async Task Done()
    {
        IsStartAppLaunch = true;
        if (MainViewModel.userInfo.thisAutoLaunch)
        {
            await launchApp();
        }
        Application.Current?.CloseWindow(Application.Current.MainPage.Window);
    }

    [RelayCommand]
    private async Task Again()
    {
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    public Task startTask()
    {
        return Task.Factory.StartNew(() => start_Adb());
    }

    public async Task start_Adb()
    {
        string cmd_output = "";
        string cmd_error = "";
        Process cmdProcess = new Process();
        ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
        try
        {
            string adbInstall = $"adb -s localhost:{MainViewModel.userInfo.thisWsaPort} install {installFilePath}";

            int waitTime_wsa = 2000;
            while (!cmd_output.Contains("Success"))
            {
                await Task.Delay(waitTime_wsa);
                waitTime_wsa = waitTime_wsa + 1000;

                cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                cmdStartInfo.RedirectStandardOutput = true;
                cmdStartInfo.RedirectStandardError = true;
                cmdStartInfo.RedirectStandardInput = true;
                cmdStartInfo.UseShellExecute = false;
                cmdStartInfo.CreateNoWindow = true;
                cmdProcess.StartInfo = cmdStartInfo;

                cmdProcess.OutputDataReceived += (s, e) => cmd_output += (
                        e.Data != null &&
                        (e.Data.Contains("C:") || e.Data.Contains("Microsoft Windows") || e.Data.Contains("Microsoft Corporation"))
                        ) ? "" : e.Data + "\n";
                cmdProcess.ErrorDataReceived += (s, e) => cmd_error += (
                        e.Data != null &&
                        e.Data.Contains("C:")) ? "" : e.Data + "\n";

                cmdProcess.EnableRaisingEvents = true;
                cmdProcess.Start();
                cmdProcess.BeginOutputReadLine();
                cmdProcess.BeginErrorReadLine();

                cmdProcess.StandardInput.WriteLine("adb start-server");

                cmdProcess.StandardInput.WriteLine(adbInstall);
                cmdProcess.StandardInput.WriteLine("adb devices");

                cmdProcess.StandardInput.WriteLine("exit");
                cmdProcess.WaitForExit(30000);

                if (waitTime_wsa > 30000)
                {
                    throw new Exception();
                }
            }
            cmdProcess.Kill();
            string rcInfo = remove_cache();
            InstallStatus = $"Successfully installed!\n\nCMD OUTPUT:{cmd_output}\n{rcInfo}";
            IsBusy = false;
            CanClick = true;
            ShowInfo = true;
        }
        catch (Exception ex)
        {            
            cmdProcess.Kill();
            string rcInfo = remove_cache();
            InstallStatus = $"Installation failed!\n\nCMD OUTPUT:\n{cmd_output}\nCMD ERROR:\n{cmd_error}\nERROR:\n{ex.Message}\n{rcInfo}";
            IsBusy = false;
            CanClick = false;
            ShowInfo = true;
        }
    }

    private async Task launchApp()
    {
        string aaptPath = Path.Combine(FileSystem.AppDataDirectory, "aapt.exe");
        string packageCmd = $"for /f \"tokens=2 delims='\" %i  in ('{aaptPath} dump badging {installFilePath} ^| findstr -n \"package: name='com\" ^| findstr \"1:\"') do @echo %i";
        await Task.Run(() =>
        {
            start_cmd(packageCmd, true);
        });
        string appLaunch = $"adb -s localhost:{MainViewModel.userInfo.thisWsaPort} shell monkey -p {appLaunchCmdOut} -c android.intent.category.LAUNCHER 1 >nul 2>nul";
        await Task.Run(() =>
        {
            start_cmd(appLaunch);
        });
    }

    private void start_cmd(string adbTaskRun, bool replaceCmdOut = false)
    {
        Process cmdProcess = new Process();
        ProcessStartInfo cmdStartInfo = new ProcessStartInfo();

        cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
        cmdStartInfo.RedirectStandardOutput = true;
        cmdStartInfo.RedirectStandardError = true;
        cmdStartInfo.RedirectStandardInput = true;
        cmdStartInfo.UseShellExecute = false;
        cmdStartInfo.CreateNoWindow = true;
        cmdProcess.StartInfo = cmdStartInfo;

        cmdProcess.OutputDataReceived += (s, e) => appLaunchCmdOut += (
                        e.Data != null && 
                        (e.Data.Contains("C:") || e.Data.Contains("Microsoft Windows") || e.Data.Contains("Microsoft Corporation"))
                        ) ? "" : e.Data;

        cmdProcess.EnableRaisingEvents = true;
        cmdProcess.Start();
        cmdProcess.BeginOutputReadLine();

        cmdProcess.StandardInput.WriteLine(adbTaskRun);

        cmdProcess.StandardInput.WriteLine("exit");
        cmdProcess.WaitForExit(3000);

        if (replaceCmdOut)
        {
            Regex.Replace(appLaunchCmdOut, @"\s+", "");
        }
        cmdProcess.Kill();
    }

    private string remove_cache()
    {
        try
        {
            if (installFilePath.Contains(FileSystem.Current.CacheDirectory))
            {
                File.Delete(installFilePath);
                return $"Successfully removed downloaded cache file: {installFilePath}";
            }
            return "";
        }
        catch (IOException ex)
        {
            return $"Unable to remove downloaded cache file: {installFilePath}\n{ex}";
        }
    }
}
