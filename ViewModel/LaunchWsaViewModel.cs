using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Net.Sockets;

namespace ApkInstallerForWindows.ViewModel;

public partial class LaunchWsaViewModel : ObservableObject
{
    public LaunchWsaViewModel()
    {
        LaunchStatus = null;
        CanClick = false;
        IsBusy = true; 
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    bool canClick;

    [ObservableProperty]
    String launchStatus;

    [RelayCommand]
    async Task Next()
    {
        await Shell.Current.GoToAsync(nameof(StartAdbPage));
    }

    public Task startTask()
    {
        return Task.Factory.StartNew(() => start_WSA());
    }

    private async Task start_WSA()
    {
        Process p = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = true;
        startInfo.FileName = @"shell:appsfolder\MicrosoftCorporationII.WindowsSubsystemForAndroid_8wekyb3d8bbwe!App";
        try
        {
            p.StartInfo = startInfo;
            p.Start();

            int waitTime_wsa = 2000;
            while (Process.GetProcessesByName("WsaClient").Length == 0)
            {
                await Task.Delay(waitTime_wsa);
                waitTime_wsa = waitTime_wsa + 1000;
                if (waitTime_wsa > 10000)
                {
                    throw new Exception();
                }
            }

            int waitTime_port = 2000;
            TimeSpan checkPortTime = TimeSpan.FromSeconds(30000);
            while (IsPortOpen("localhost", Int32.Parse(MainViewModel.userInfo.thisWsaPort), checkPortTime) == false)
            {
                await Task.Delay(waitTime_wsa);
                waitTime_port = waitTime_port + 1000;
                if (waitTime_port > 10000)
                {
                    throw new Exception();
                }
            }
            await IsAdbConnect();
            IsBusy = false;  
            CanClick = true;
        }
        catch (Exception ex)
        {
            LaunchStatus = "Timeout. Fail to Launch WSA. Please check if you can manually open WSA.";
            IsBusy = false;
            CanClick = false;
            await Application.Current.MainPage.DisplayAlert("Installation failed", $"Cancelled. Unable to launch WSA. {ex.Message}", "OK");
        }
    }

    private bool IsPortOpen(string host, int port, TimeSpan timeout)
    {
        try
        {
            using (var client = new TcpClient())
            {
                var result = client.BeginConnect(host, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(timeout);
                client.EndConnect(result);
                return success;
            }
        }
        catch
        {
            return false;
        }
    }

    private async Task IsAdbConnect()
    {
        string cmd_output = "";
        string cmd_error = "";
        try
        {
            Process cmdProcess = new Process();
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo();

            int waitTime_wsa = 2000;
            while (!cmd_output.Contains("connected"))
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
                cmdProcess.StandardInput.WriteLine($"adb connect localhost:{MainViewModel.userInfo.thisWsaPort}");

                cmdProcess.StandardInput.WriteLine("exit");
                cmdProcess.WaitForExit(30000);

                if (waitTime_wsa > 30000)
                {
                    throw new Exception();
                }
            }
            LaunchStatus = "Successfully launched WSA and connected to Adb";
        }
        catch (Exception ex)
        {
            LaunchStatus = "Unable to connect with Adb";
            CanClick = false;
            await Application.Current.MainPage.DisplayAlert("Installation failed", $"{cmd_output}\n{cmd_error}\nERROR:{ex}", "OK");
        }
    }

}
