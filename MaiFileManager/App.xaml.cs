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
#if ANDROID
                AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = AndroidX.AppCompat.App.AppCompatDelegate.ModeNightFollowSystem;
#endif
                Current.UserAppTheme = AppTheme.Unspecified;
                break;
            case 1:
#if ANDROID
                AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = AndroidX.AppCompat.App.AppCompatDelegate.ModeNightNo;
#endif
                Current.UserAppTheme = AppTheme.Light;
                break;
            case 2:
#if ANDROID
                AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = AndroidX.AppCompat.App.AppCompatDelegate.ModeNightYes;
#endif
                Current.UserAppTheme = AppTheme.Dark;
                break;
        }
        InitializeComponent();
        MainPage = new AppShell();
	}
}
