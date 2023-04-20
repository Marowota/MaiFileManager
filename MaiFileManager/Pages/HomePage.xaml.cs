using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MaiFileManager.Classes;
using MaiFileManager.Services;

namespace MaiFileManager.Pages;

public partial class HomePage : ContentPage
{
    public FileList FileListObj { get; set; } = new FileList();
    bool IsSelectionMode { get; set; } = false;
    bool IsCheckedBySelect { get; set; } = false;
    bool IsAutoChecked { get; set; } = true;
    public HomePage()
    {
        InitializeComponent();
    }
    //check if checkbox changed as user want
    private void CheckedBySelectChange(FileSystemInfoWithIcon tmp, bool? value = null)
    {
        IsCheckedBySelect = true;
        tmp.CheckBoxSelected = value ?? (tmp.CheckBoxSelected ? false : true);
        IsCheckedBySelect = false;
        CheckAllChkValueCondition(tmp);
    }

    private void CheckAllChkValueCondition(FileSystemInfoWithIcon tmp)
    {
        if (!tmp.CheckBoxSelected)
        {
            IsAutoChecked = false;
            SelectAllChk.IsChecked = false;
            IsAutoChecked = true;
        }
    }

    private void AutoCheck(bool value)
    {
        if (IsAutoChecked)
        {
            foreach (FileSystemInfoWithIcon f in FileListObj.CurrentFileList)
            {
                IsCheckedBySelect = true;
                f.CheckBoxSelected = value;
                IsCheckedBySelect = false;
            }
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
        IsSelectionMode = value;
        BackButton.IsVisible = value ? false : (FileListObj.BackDeep > 0);
    }

    private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsSelectionMode)
        {
            if (e.CurrentSelection.Count == 0) return;
            FileSystemInfoWithIcon tmp = e.CurrentSelection.FirstOrDefault() as FileSystemInfoWithIcon;
            CheckedBySelectChange(tmp);
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
        (sender as SwipeView).Close();
        if (!IsSelectionMode)
        {
            SelecitonMode(true);
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!IsCheckedBySelect)
        {
            (sender as CheckBox).IsChecked = (e.Value ? false : true);
            IsCheckedBySelect = true;
        }
    }

    private void SelectAllChk_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        AutoCheck(e.Value);
    }

    private void CancleMultipleSelection_Clicked(object sender, EventArgs e)
    {
        foreach (FileSystemInfoWithIcon f in FileListObj.CurrentFileList)
        {
            CheckedBySelectChange(f, false);
        }
        SelecitonMode(false);
    }

}