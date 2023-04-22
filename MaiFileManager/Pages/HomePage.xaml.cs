using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MaiFileManager.Classes;
using MaiFileManager.Services;
using Org.W3c.Dom.LS;

namespace MaiFileManager.Pages;

public partial class HomePage : ContentPage
{
    public FileList FileListObj { get; set; } = new FileList();
    bool IsAutoChecked { get; set; } = true;
    public HomePage()
    {
        InitializeComponent();
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

    private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FileListObj.IsSelectionMode)
        {
            if (e.CurrentSelection.Count == 0) return;
            FileSystemInfoWithIcon tmp = e.CurrentSelection.FirstOrDefault() as FileSystemInfoWithIcon;
            CheckedBySelectChange(tmp);
            CheckAllChkValueCondition();
        }
        else
        {
            FileListObj.PathSelection(sender, e);
            BackButton.IsVisible = (FileListObj.BackDeep > 0);
        }
        (sender as CollectionView).SelectedItem = null;
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        if (FileListObj.IsSelectionMode) return;
        FileListObj.Back(sender, e);
        BackButton.IsVisible = (FileListObj.BackDeep > 0);
    }

    protected override bool OnBackButtonPressed()
    {
        if (FileListObj.BackDeep > 0)
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

    private void Paste_Clicked(object sender, EventArgs e)
    {
        FileListObj.PasteMode();
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        bool result = await Shell.Current.DisplayAlert("Delete File",
                                                       "Are you sure you want to delete these file\nThis can't be undone",
                                                       "Yes",
                                                       "No");
        if (result)
        {
            FileListObj.DeleteMode();
        }
    }
}