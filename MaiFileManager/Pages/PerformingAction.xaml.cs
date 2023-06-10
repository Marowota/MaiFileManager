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
    System.Timers.Timer t = new System.Timers.Timer(500);

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
                await currentFileListObj.DeleteModeAsync(); 
                await this.Dispatcher.DispatchAsync(() => {
                    lblType.Text = "Deleted";
                });
                break;
        }
        currentFileListObj.NavigatedPage = null;
    }
}