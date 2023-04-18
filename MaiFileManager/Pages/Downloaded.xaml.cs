using MaiFileManager.Classes;
using System.Collections.ObjectModel;
using MaiFileManager.Services;

namespace MaiFileManager.Pages;

public partial class Downloaded : ContentPage
{
    public FileList FileListObj { get; set; } = new FileList(1);
    public Downloaded()
	{
		InitializeComponent();
    }

    private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FileListObj.PathSelection(sender, e);
        BackButton.IsVisible = (FileListObj.BackDeep > 0);
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
}