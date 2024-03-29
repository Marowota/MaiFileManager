using MaiFileManager.Classes;

namespace MaiFileManager.Pages;

public partial class Setting : ContentPage
{
    private bool IsFirstLoad = false;
	public Setting()
	{
		InitializeComponent();
        InitLoad();
	}

    void InitLoad()
    {
        int rd = Preferences.Default.Get("Sort_by", 0);
        int theme = Preferences.Default.Get("Theme", 0);
        IsFirstLoad = true;
        switch (theme)
        {
            case 0: DefaultRd.IsChecked = true; break;
            case 1: LightRd.IsChecked = true; break;
            case 2: DarkRd.IsChecked = true; break;
        }
        switch (rd)
        {
            case 0: NameAZ.IsChecked = true; break;
            case 1: NameZA.IsChecked = true; break;
            case 2: SizeSL.IsChecked = true; break;
            case 3: SizeLS.IsChecked = true; break;
            case 4: TypeAZ.IsChecked = true; break;
            case 5: TypeZA.IsChecked = true; break;
            case 6: DateNO.IsChecked = true; break;
            case 7: DateON.IsChecked = true; break;
        }
    }

    private async void DefaultRd_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if ((sender as RadioButton).IsChecked)
        {
            if (!IsFirstLoad)
            {
                bool allow = await DisplayAlert("Theme change", "The application must restart to apply theme change, continue?", "OK", "Cancel");
                if (!allow)
                {
                    InitLoad();
                    IsFirstLoad = false;
                    return;
                }
            }
            Preferences.Default.Set("Theme", 0);
            Application.Current.UserAppTheme = AppTheme.Unspecified;
#if ANDROID
            AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = AndroidX.AppCompat.App.AppCompatDelegate.ModeNightFollowSystem;
#endif
            if (!IsFirstLoad)
            {
                Application.Current.MainPage = new AppShell();
            }
            IsFirstLoad = false;
        }
    }

    private async void LightRd_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if ((sender as RadioButton).IsChecked)
        {
            if (!IsFirstLoad)
            {
                bool allow = await DisplayAlert("Theme change", "The application must restart to apply theme change, continue?", "OK", "Cancel");
                if (!allow)
                {
                    InitLoad();
                    IsFirstLoad = false;
                    return;
                }
            }
            Preferences.Default.Set("Theme", 1);
            Application.Current.UserAppTheme = AppTheme.Light;
#if ANDROID
            AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = AndroidX.AppCompat.App.AppCompatDelegate.ModeNightNo;
#endif
            if (!IsFirstLoad)
            {
                Application.Current.MainPage = new AppShell();
            }
            IsFirstLoad = false;
        }
    }

    private async void DarkRd_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if ((sender as RadioButton).IsChecked)
        {
            if (!IsFirstLoad)
            {
                bool allow = await DisplayAlert("Theme change", "The application must restart to apply theme change, continue?", "OK", "Cancel");
                if (!allow)
                {
                    InitLoad();
                    IsFirstLoad = false;
                    return;
                }
            }
            Preferences.Default.Set("Theme", 2);
            Application.Current.UserAppTheme = AppTheme.Dark;
#if ANDROID
            AndroidX.AppCompat.App.AppCompatDelegate.DefaultNightMode = AndroidX.AppCompat.App.AppCompatDelegate.ModeNightYes;
#endif
            if (!IsFirstLoad)
            {
                Application.Current.MainPage = new AppShell();
            }
            IsFirstLoad = false;
        }
    }

    private void NameAZ_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 0);
    }

    private void NameZA_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 1);
    }

    private void SizeSL_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 2);
    }

    private void SizeLS_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 3);
    }

    private void TypeAZ_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 4);
    }

    private void TypeZA_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 5);
    }

    private void DateNO_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 6);
    }

    private void DateON_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Preferences.Default.Set("Sort_by", 7);
    }


}