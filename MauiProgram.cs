using ApkInstallerForWindows.ViewModel;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace ApkInstallerForWindows;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<ApkInstallPage>();
        builder.Services.AddTransient<ApkInstallViewModel>();

        builder.Services.AddTransient<LaunchWsaPage>();
        builder.Services.AddTransient<LaunchWsaViewModel>();

        builder.Services.AddTransient<StartAdbPage>();
        builder.Services.AddTransient<StartAdbViewModel>();

        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<AppcenterDownloadPage>();
        builder.Services.AddTransient<AppcenterDownloadViewModel>();

        return builder.Build();
    }
}
