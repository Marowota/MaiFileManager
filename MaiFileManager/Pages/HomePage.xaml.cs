using System.Collections.ObjectModel;
using System.Diagnostics;
using MaiFileManager.Classes;
using MaiFileManager.Services;

namespace MaiFileManager.Pages;

public partial class HomePage : ContentPage
{
	public ObservableCollection<FileSystemInfoWithIcon> FileList { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();
    public FileManager fileManager { get; set; } = new FileManager();
    int BackDeep;
    public HomePage()
    {
        InitializeComponent();
        this.Loaded += HomePage_Loaded;
        BackDeep = 0;
    }

    private async void HomePage_Loaded(object sender, EventArgs e)
    {
        bool accepted = await RequestPermAsync();
        if (!accepted)
        {
            Application.Current.Quit();
        }
        UpdateFileList();
    }
    #region Permission
    private async Task<bool> RequestPermAsync()
    {
        var statusR = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        var statusW = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (statusR != PermissionStatus.Granted)
        {
            statusR = await Permissions.RequestAsync<Permissions.StorageRead>();
        }
        if (statusW != PermissionStatus.Granted)
        {
            statusW = await Permissions.RequestAsync<Permissions.StorageWrite>();
        }

        if (statusW != PermissionStatus.Granted || statusR != PermissionStatus.Granted)
        {
            await Shell.Current.DisplayAlert("Permission not granted", "Need storage permission to use the app", "OK");
            return false;
        }
        return true;
    }
    #endregion
    private void UpdateFileList()
    {
        FileList.Clear();
        foreach (FileSystemInfo info in fileManager.GetListFile())
        {
            if (info.GetType() == typeof(FileInfo))
            {
                FileList.Add(new FileSystemInfoWithIcon(info, MaiIcon.GetIcon(info.Extension), 40));
            }
            else if  (info.GetType() == typeof(DirectoryInfo))
            {
                FileList.Add(new FileSystemInfoWithIcon(info, "folder.png", 45));
            }
        }
    }
    private async void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is null) return;
        FileSystemInfoWithIcon selectedWIcon = e.CurrentSelection.FirstOrDefault() as FileSystemInfoWithIcon;
        FileSystemInfo selected = selectedWIcon.fileInfo;
        if (selected.GetType() == typeof(FileInfo))
        {
            FileInfo file = (FileInfo) selected;
            await Launcher.OpenAsync(new OpenFileRequest("Open File", new ReadOnlyFile(selected.FullName)));
        }
        if (selected.GetType() == typeof(DirectoryInfo))
        {
            if (selected == null)
                return;
            fileManager.UpdateDir(selected.FullName);
            UpdateFileList();
            UpdateBackButtonVisible(1);
        }
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        fileManager.BackDir();
        UpdateFileList();
        UpdateBackButtonVisible(-1);
    }

    //Update BackButtonVisability when user redirect
    void UpdateBackButtonVisible(int val)
    {
        BackDeep += val;
        if (BackDeep < 0)
        {
            BackDeep = 0;
        }
        BackButton.IsVisible = (BackDeep > 0);
    }
}