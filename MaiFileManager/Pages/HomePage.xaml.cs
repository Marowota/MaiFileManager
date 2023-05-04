using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MaiFileManager.Classes;
using MaiFileManager.Services;
using Org.W3c.Dom.LS;

namespace MaiFileManager.Pages;

public partial class HomePage : ContentPage
{
    public FileList FileListObj { get; set; }
    bool IsAutoChecked { get; set; } = true;
    bool IsRenameMode { get; set; } = false;
    bool FirstLoad = true;
    public HomePage()
    {
        FileListObj = new FileList();
        InitializeComponent();
        InitialLoad();
    }
    public HomePage(int param)
    {
        FileListObj = new FileList(param);
        InitializeComponent();
        InitialLoad();
    }
    private async void InitialLoad()
    {
        await FileListObj.InitialLoadAsync();
    }
    //check if checkbox changed as user want
    private void CheckedBySelectChange(FileSystemInfoWithIcon tmp, bool? value = null)
    {
        FileListObj.NumberOfCheked += (tmp.CheckBoxSelected ? 0 : 1);
        tmp.CheckBoxSelected = value ?? (tmp.CheckBoxSelected ? false : true);
        FileListObj.NumberOfCheked += (tmp.CheckBoxSelected ? 0 : -1);
    }

    private void CheckAllChkValueCondition()
    {
        if (FileListObj.NumberOfCheked == FileListObj.CurrentFileList.Count)
        {
            if (SelectAllChk.IsChecked != true)
            {
                IsAutoChecked = false;
            }
            SelectAllChk.IsChecked = true;
        }
        else
        {
            if (SelectAllChk.IsChecked != false)
            {
                IsAutoChecked = false;
            }
            SelectAllChk.IsChecked = false;
        }
    }

    private void SelecitonMode(bool value)
    {
        foreach (FileSystemInfoWithIcon f in FileListObj.CurrentFileList)
        {
            f.CheckBoxSelectVisible = value;
        }
        CancleMultipleSelection.IsVisible = value;
        SelectAllChk.IsVisible = value;
        OptionBar.IsVisible = value;
        FileListObj.IsSelectionMode = value;
        BackButton.IsVisible = value ? false : (FileListObj.BackDeep > 0);
        CheckAllChkValueCondition();
    }

    private async void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FileListObj.IsSelectionMode)
        {
            if (e.CurrentSelection.Count == 0) return;
            FileSystemInfoWithIcon tmp = e.CurrentSelection.FirstOrDefault() as FileSystemInfoWithIcon;
            CheckedBySelectChange(tmp);
            CheckAllChkValueCondition();
        }
        else if (IsRenameMode)
        {
            IsRenameMode = false;
        }
        else
        {
            await FileListObj.PathSelectionAsync(sender, e);
            BackButton.IsVisible = (FileListObj.BackDeep > 0);
        }
        (sender as CollectionView).SelectedItem = null;
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        if (FileListObj.IsSelectionMode) return;
        if (FileListObj.BackDeep == 0) return;
        await FileListObj.BackAsync(sender, e);
        BackButton.IsVisible = (FileListObj.BackDeep > 0);
    }

    protected override bool OnBackButtonPressed()
    {
        if (FileListObj.IsSelectionMode)
        {
            CancleMultipleSelection_Clicked(null, null);
        }
        else if (FileListObj.BackDeep > 0)
        {
            BackButton_Clicked(BackButton, EventArgs.Empty);
        }
        else
        {
            base.OnBackButtonPressed();
        }
        return true;
    }

    private void SwipeView_SwipeStarted(object sender, SwipeStartedEventArgs e)
    {
        if (!FileListObj.IsSelectionMode)
        {
            SelecitonMode(true);
        }
        (sender as SwipeView).Close();
    }

    private void CheckBox_Focused(object sender, FocusEventArgs e)
    {
        CheckBox tmp = sender as CheckBox;
        tmp.IsChecked = !tmp.IsChecked;
        tmp.Unfocus();
    }

    private void SelectAllChk_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (IsAutoChecked)
        {
            foreach (FileSystemInfoWithIcon f in FileListObj.CurrentFileList)
            {
                CheckedBySelectChange(f, e.Value);
            }
        }
        else
        {
            IsAutoChecked = true;
        }
    }

    private void CancleMultipleSelection_Clicked(object sender, EventArgs e)
    {
        foreach (FileSystemInfoWithIcon f in FileListObj.CurrentFileList)
        {
            CheckedBySelectChange(f, false);
        }
        SelecitonMode(false);
    }

    private void Cut_Clicked(object sender, EventArgs e)
    {
        FileListObj.ModifyMode(FileList.FileSelectOption.Cut);
    }

    private void Copy_Clicked(object sender, EventArgs e)
    {
        FileListObj.ModifyMode(FileList.FileSelectOption.Copy);
    }

    private async void Paste_Clicked(object sender, EventArgs e)
    {
        await FileListObj.PasteModeAsync();
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        bool result = await Shell.Current.DisplayAlert("Delete File",
                                                       "Are you sure you want to delete these file\nThis can't be undone",
                                                       "Yes",
                                                       "No");
        if (result)
        {
            await FileListObj.DeleteModeAsync();
        }
    }

    private async void Rename_SwipeStarted(object sender, SwipeStartedEventArgs e)
    {
        IsRenameMode = true;
        string path = ((sender as SwipeView).RightItems.First() as SwipeItem).Text;
        string result = await Shell.Current.DisplayPromptAsync("Rename", "Renaming:"
                                                                       + Path.GetFileName(path)
                                                                       + "\nType new file name:\n");
        while (result == "")
        {
            await Shell.Current.DisplayAlert("Invalid", "Please type a name", "OK");
            result = await Shell.Current.DisplayPromptAsync("Rename", "Renaming:"
                                                                    + Path.GetFileName(path)
                                                                    + "\nType new file name:\n");
        }
        if (result != null)
        {
            await FileListObj.RenameModeAsync(path, result);
        }    
        (sender as SwipeView).Close(false);
    }

    private async void RefreshView_Refreshing(object sender, EventArgs e)
    {
        SwipeFileBox.IsVisible = false;
        await Task.Run(FileListObj.UpdateFileListAsync);
        SwipeFileBox.IsVisible = true;
        (sender as RefreshView).IsRefreshing = false;
    }

    private async void homePage_Appearing(object sender, EventArgs e)
    {
        if (!FirstLoad)
        {
            await Task.Run(FileListObj.UpdateFileListAsync);
        }
        FirstLoad = false;
    }

    private void SearchFileFolder_Clicked(object sender, EventArgs e)
    {

    }

    private async void AddNewFolder_Clicked(object sender, EventArgs e)
    {
        bool success = false;
        do
        {
            string result = await Shell.Current.DisplayPromptAsync("New Folder", "Folder name: \n");
            while (result == "")
            {
                await Shell.Current.DisplayAlert("Invalid", "Please type a name", "OK");
                result = await Shell.Current.DisplayPromptAsync("New Folder", "Folder name: \n");
            }
            if (result != null)
            {
                success = await FileListObj.NewFolderAsync(result);
            }
            else
            {
                return;
            }
        } while (!success);
    }
}