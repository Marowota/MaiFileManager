using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaiFileManager.Services;

namespace MaiFileManager.Classes
{
    /// <summary>
    /// Class help the UI get the information to view the list of files
    /// </summary>
    public class FileList
    {
        public ObservableCollection<FileSystemInfoWithIcon> CurrentFileList { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();
        public FileManager CurrentDirectoryInfo { get; set; }
        public int BackDeep {get; set;} = 0;
        public FileList()
        {
            CurrentDirectoryInfo = new FileManager(); 
            InitialLoad();
        }
        public FileList(int type)
        {
            CurrentDirectoryInfo = new FileManager(type);
            InitialLoad();
        }
        public FileList(string path)
        {
            CurrentDirectoryInfo = new FileManager(path); 
            InitialLoad();
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

        private async void InitialLoad()
        {
            bool accepted = await RequestPermAsync();
            if (!accepted)
            {
                Application.Current.Quit();
            }
            UpdateFileList();
        }

        private void UpdateBackDeep(int val)
        {
            BackDeep += val;
            if (BackDeep < 0)
            {
                BackDeep = 0;
            }
        }

        internal void UpdateFileList()
        {
            CurrentFileList.Clear();
            foreach (FileSystemInfo info in CurrentDirectoryInfo.GetListFile())
            {
                if (info.GetType() == typeof(FileInfo))
                {
                    CurrentFileList.Add(new FileSystemInfoWithIcon(info, MaiIcon.GetIcon(info.Extension), 40));
                }
                else if (info.GetType() == typeof(DirectoryInfo))
                {
                    CurrentFileList.Add(new FileSystemInfoWithIcon(info, "folder.png", 45));
                }
            }
        }

        internal async void PathSelection(object sender, SelectionChangedEventArgs e)
        {
            if (sender is null) return;
            if (e.CurrentSelection.Count == 0) return;
            FileSystemInfoWithIcon selectedWIcon = e.CurrentSelection.FirstOrDefault() as FileSystemInfoWithIcon;
            FileSystemInfo selected = selectedWIcon.fileInfo;
            if (selected.GetType() == typeof(FileInfo))
            {
                FileInfo file = (FileInfo)selected;
                await Launcher.OpenAsync(new OpenFileRequest("Open File", new ReadOnlyFile(selected.FullName)));
            }
            if (selected.GetType() == typeof(DirectoryInfo))
            {
                if (selected == null)
                    return;
                CurrentDirectoryInfo.UpdateDir(selected.FullName);
                UpdateFileList();
                UpdateBackDeep(1);
            }
        }

        internal void Back(object sender, EventArgs e)
        {
            CurrentDirectoryInfo.BackDir();
            UpdateFileList();
            UpdateBackDeep(-1);
        }

    }
}
