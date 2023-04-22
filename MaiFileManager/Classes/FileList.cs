using MaiFileManager.Services;
using System.Collections.ObjectModel;

namespace MaiFileManager.Classes
{
    /// <summary>
    /// Class help the UI get the information to view the list of files
    /// </summary>
    public class FileList
    {
        public enum FileSelectOption
        {
            None,
            Cut, 
            Copy,
        }
        public ObservableCollection<FileSystemInfoWithIcon> CurrentFileList { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();
        public FileManager CurrentDirectoryInfo { get; set; }
        public int BackDeep {get; set;} = 0;
        public FileSelectOption OperatedOption { get; set; } = FileSelectOption.None;
        public ObservableCollection<FileSystemInfoWithIcon> OperatedFileList { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();
        public bool IsSelectionMode { get; set; } = false;
        public int NumberOfCheked { get; set; } = 0;
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

            var statusW = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (statusW != PermissionStatus.Granted)
            {
                statusW = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }
            if (statusW != PermissionStatus.Granted)
            {
                return CurrentDirectoryInfo.GetPerm();
            }
            return true;
        }
        #endregion

        private async void InitialLoad()
        {
            bool accepted = await RequestPermAsync();
            if (!accepted)
            {
                await Shell.Current.DisplayAlert("Permission not granted", "Need storage permission to use the app", "OK");
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
            if (IsSelectionMode)
            {
                foreach (FileSystemInfoWithIcon f in CurrentFileList)
                {
                    f.CheckBoxSelectVisible = true;
                }
            }
            NumberOfCheked = 0;
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
        static void CopyDirectory(DirectoryInfo sourceDir, string destinationPath)
        {
            if (!sourceDir.Exists)
            {
                return;
            }

            List<DirectoryInfo> sourceDirTemp = new List<DirectoryInfo>();
            foreach (DirectoryInfo directory in sourceDir.GetDirectories())
            {
                sourceDirTemp.Add(directory);
            }

            string newSourcePath = Path.Combine(destinationPath, sourceDir.Name);
            DirectoryInfo newSourceDir = Directory.CreateDirectory(newSourcePath);
            foreach (FileInfo file in sourceDir.GetFiles())
            {
                file.CopyTo(Path.Combine(newSourcePath, file.Name));
            }

            foreach (DirectoryInfo directory in sourceDirTemp)
            {
                CopyDirectory(directory, newSourcePath);
            }

        }

        internal void ModifyMode(FileSelectOption mode)
        {
            OperatedOption = mode;
            OperatedFileList.Clear();
            foreach (FileSystemInfoWithIcon f in CurrentFileList)
            {
                if (f.CheckBoxSelected)
                {
                    OperatedFileList.Add(f);
                }
            }
        }

        internal void DeleteMode()
        {
            foreach (FileSystemInfoWithIcon f in CurrentFileList)
            {
                if (f.CheckBoxSelected)
                {
                    f.fileInfo.Delete();
                }
            }
            UpdateFileList();
        }


        internal void PasteMode()
        {
            foreach (FileSystemInfoWithIcon f in OperatedFileList)
            {
                switch (OperatedOption)
                {
                    case FileSelectOption.Cut:
                        {
                            if (f.fileInfo.GetType() == typeof(FileInfo))
                            {
                                string targetFilePath = Path.Combine(CurrentDirectoryInfo.CurrentDir, f.fileInfo.Name);
                                (f.fileInfo as FileInfo).MoveTo(targetFilePath);
                            }
                            else if (f.fileInfo.GetType() == typeof(DirectoryInfo))
                            {
                                string targetFilePath = Path.Combine(CurrentDirectoryInfo.CurrentDir, f.fileInfo.Name);
                                (f.fileInfo as DirectoryInfo).MoveTo(targetFilePath);
                            }
                            break;
                        }
                    case FileSelectOption.Copy:
                        { 
                            if (f.fileInfo.GetType() == typeof(FileInfo))
                            {
                                string targetFilePath = Path.Combine(CurrentDirectoryInfo.CurrentDir, f.fileInfo.Name);
                                (f.fileInfo as FileInfo).CopyTo(targetFilePath);
                            }
                            else if (f.fileInfo.GetType() == typeof(DirectoryInfo))
                            {
                                CopyDirectory((f.fileInfo as DirectoryInfo), CurrentDirectoryInfo.CurrentDir);
                            }
                        break;
                        }
                }
            }
            UpdateFileList();
        }
    }
}
