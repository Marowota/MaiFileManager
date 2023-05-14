using Android.Hardware.Lights;

namespace MaiFileManager;

public partial class App : Application
{
	public App()
    {
        int theme = Preferences.Default.Get("Theme", 0);
        switch (theme)
        {
            case 0:
                Current.UserAppTheme = AppTheme.Unspecified;
                break;
            case 1:
                Current.UserAppTheme = AppTheme.Light;
                break;
            case 2:
                Current.UserAppTheme = AppTheme.Dark;
                break;
        }
        InitializeComponent();
        MainPage = new AppShell();
	}
}
