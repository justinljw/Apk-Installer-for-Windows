using Microsoft.Windows.AppLifecycle;

namespace ApkInstallerForWindows;

public partial class App : Application
{
    public static string argskind { get; private set; }
    public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        var goodArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        argskind = goodArgs.Kind.ToString();

        const int newWidth = 700;
        const int newHeight = 450;

        window.Width = newWidth;
        window.Height = newHeight;

        return window;
    }
}
