using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace ApkInstallerForWindows.ViewModel;

public partial class StartAdbViewModel : ObservableObject
{
    public StartAdbViewModel()
    {
        InstallStatus = null;
        CanClick = false;
        IsBusy = true;
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    bool canClick;

    [ObservableProperty]
    String installStatus;

    [RelayCommand]
    void Done()
    {
        Application.Current?.CloseWindow(Application.Current.MainPage.Window);
    }

    public Task startTask()
    {
        return Task.Factory.StartNew(() => start_Adb());
    }

    public async Task start_Adb()
    {
        string cmd_output = "";
        string cmd_error = "";
        try
        {
            string installFilePath = MainViewModel.apkFilePath != null && MainViewModel.apkFilePath != ""
                                         ? MainViewModel.apkFilePath : AppcenterDownloadViewModel.savedFilePath;
            string adbInstall = $"adb -s localhost:{MainViewModel.userInfo.thisWsaPort} install {installFilePath}";

            Process cmdProcess = new Process();
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo();

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

                cmdProcess.OutputDataReceived += (s, e) => cmd_output += (e.Data != null && e.Data.Contains("C:")) ? "" : (e.Data + "\n");
                cmdProcess.ErrorDataReceived += (s, e) => cmd_error += (e.Data != null && e.Data.Contains("C:")) ? "" : (e.Data + "\n");

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
            IsBusy = false;
            InstallStatus = cmd_output;
            CanClick = true;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            CanClick = false;
            InstallStatus = $"Installation failed!\n{cmd_output}\n{cmd_error}\nERROR:{ex}";
        }
    }
}
