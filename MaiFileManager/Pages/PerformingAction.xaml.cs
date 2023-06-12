using MaiFileManager.Classes;
using System.Timers;
using static Java.Util.Jar.Attributes;

namespace MaiFileManager.Pages;

[QueryProperty(nameof(curretAt), "atype")]
[QueryProperty(nameof(currentFileListObj), "flobj")]
public partial class PerformingAction : ContentPage
{
    public enum ActionType
    {
        paste,
        delete
    }

    public FileList currentFileListObj { get; set; } = null;
    ActionType curretAt { get; set; }
    System.Timers.Timer t = new System.Timers.Timer(400);
    private FileSystemInfoWithIcon currentProcessingFile;
    public FileSystemInfoWithIcon CurrentProcessingFile
    {
        get
        {
            return currentProcessingFile;
        }
        set
        {
            OnPropertyChanging(nameof(CurrentProcessingFile));
            currentProcessingFile = value;
            OnPropertyChanged(nameof(CurrentProcessingFile));
        }
    }
    bool IsCompleted { get; set; } = false;

    public PerformingAction()
    {
        InitializeComponent(); 
        t.Elapsed += T_Elapsed;
        t.Start();
    }

    public PerformingAction(FileList fileListObj, ActionType at)
    {
        currentFileListObj = fileListObj;
        curretAt = at;
        InitializeComponent();
        t.Elapsed += T_Elapsed;
        t.Start();
    }

    private async void T_Elapsed(object sender, ElapsedEventArgs e)
    {
        t.Stop();
        currentFileListObj.NavigatedPage = this;
        if (currentFileListObj == null)
        {
            return;
        }
        System.Timers.Timer reloadTimer = new System.Timers.Timer(300);
        reloadTimer.Elapsed += ReloadTimer_Elapsed;
        reloadTimer.Start();
        switch (curretAt)
        {
            case ActionType.paste:
                await this.Dispatcher.DispatchAsync( () => {
                    lblType.Text = (currentFileListObj.OperatedOption == FileList.FileSelectOption.Cut ? "Moving..." :
                                                  currentFileListObj.OperatedOption == FileList.FileSelectOption.Copy ? "Copying..." : "Do nothing...");
                });
              
                await currentFileListObj.PasteModeAsync(); 
                
                await this.Dispatcher.DispatchAsync(() => {
                    lblType.Text = (currentFileListObj.OperatedOption == FileList.FileSelectOption.Cut ? "Moved" :
                                currentFileListObj.OperatedOption == FileList.FileSelectOption.Copy ? "Copied" : "Did nothing!");
                });
                break;
            case ActionType.delete:
                await this.Dispatcher.DispatchAsync(() => {
                    lblType.Text = "Deleting...";
                });
                    int result = await currentFileListObj.DeleteModeAsync(); 
                await this.Dispatcher.DispatchAsync(() => {
                    if (result == 0)
                    {
                        lblType.Text = "Did nothing!";
                    }
                    if (result == 1)
                    {
                        lblType.Text = "Deleted";
                    }
                });
                break;
        }
        reloadTimer.Stop();
        gridProcessing.IsVisible = false;
        currentFileListObj.NavigatedPage = null;
        currentProcessingFile = null;
        await this.Dispatcher.DispatchAsync(() =>
        {
            ActionButton.IsEnabled = true;
            switch (Application.Current.RequestedTheme)
            {
                case AppTheme.Unspecified:
                    ActionButton.Background = Color.FromRgb(81, 43, 212);
                    break;
                case AppTheme.Light:
                    ActionButton.Background = Color.FromRgb(81, 43, 212);
                    break;
                case AppTheme.Dark:
                    ActionButton.Background = Colors.White;
                    break;
            }
        });
    }

    private void ReloadTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        CurrentProcessingFile = currentFileListObj.OperatedFileListView.FirstOrDefault();
    }

    protected override bool OnBackButtonPressed()
    {
        if (IsCompleted)
        {
            return base.OnBackButtonPressed();
        }
        return true;
    }
    private async void ActionButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}