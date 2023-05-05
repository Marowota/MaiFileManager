using MaiFileManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Android.Icu.Text.IDNA;

namespace MaiFileManager.Classes
{
    /// <summary>
    /// Class help the UI get the information to view the list of files
    /// </summary>
    public class FileList : INotifyPropertyChanged
    {
        public enum FileSelectOption
        {
            None,
            Cut, 
            Copy,
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FileSystemInfoWithIcon> CurrentFileList { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();
        public FileManager CurrentDirectoryInfo { get; set; }
        public int BackDeep {get; set;} = 0;
        public FileSelectOption OperatedOption { get; set; } = FileSelectOption.None;
        public ObservableCollection<FileSystemInfoWithIcon> OperatedFileList { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();
        public bool IsSelectionMode { get; set; } = false;
        public int NumberOfCheked { get; set; } = 0;
        private bool isReloading = true;
        public bool IsReloading
        {
            get
            {
                return isReloading;
            }
            set
            {
                isReloading = value;
                OnPropertyChanged(nameof(IsReloading));
                OnPropertyChanged(nameof(IsNotReloading));
            }
        }
        public bool IsNotReloading
        {
            get 
            { 
                return !isReloading; 
            }
        }
        public FileList()
        {
            CurrentDirectoryInfo = new FileManager();
        }
        public FileList(int type)
        {
            CurrentDirectoryInfo = new FileManager(type);
        }
        public FileList(string path)
        {
            CurrentDirectoryInfo = new FileManager(path); 
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


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

        internal async Task InitialLoadAsync()
        {
            bool accepted = await RequestPermAsync();
            if (!accepted)
            {
                await Shell.Current.DisplayAlert("Permission not granted", "Need storage permission to use the app", "OK");
                Application.Current.Quit();
            }
            else
            {
                await Task.Run(UpdateFileListAsync);
            }    
            //await Task.Run(UpdateFileListAsync);
        }

        private void UpdateBackDeep(int val)
        {
            BackDeep += val;
            if (BackDeep < 0)
            {
                BackDeep = 0;
            }
        }

        internal async Task UpdateFileListAsync()
        {
            IsReloading = true;
            await Task.Run(async () =>
            {
                CurrentFileList.Clear();
                foreach (FileSystemInfo info in CurrentDirectoryInfo.GetListFile())
                {
                    await Task.Run(() =>
                    {
                        if (info.GetType() == typeof(FileInfo))
                        {
                            CurrentFileList.Add(new FileSystemInfoWithIcon(info, MaiIcon.GetIcon(info.Extension), 40));
                        }
                        else if (info.GetType() == typeof(DirectoryInfo))
                        {
                            CurrentFileList.Add(new FileSystemInfoWithIcon(info, "folder.png", 45));
                        }

                    });
                }
                if (IsSelectionMode)
                {
                    foreach (FileSystemInfoWithIcon f in CurrentFileList)
                    {
                        f.CheckBoxSelectVisible = true;
                    }
                }
                NumberOfCheked = 0;
            });
            IsReloading = false;
        }

        internal async Task<int> PathSelectionAsync(object sender, SelectionChangedEventArgs e)
        {
            if (sender is null) return -1;
            if (e.CurrentSelection.Count == 0) return -1;
            FileSystemInfoWithIcon selectedWIcon = e.CurrentSelection.FirstOrDefault() as FileSystemInfoWithIcon;
            FileSystemInfo selected = selectedWIcon.fileInfo;
            if (selected.GetType() == typeof(FileInfo))
            {
                FileInfo file = (FileInfo)selected;
                await Launcher.OpenAsync(new OpenFileRequest("Open File", new ReadOnlyFile(selected.FullName)));
                return 0;
            }
            else if (selected.GetType() == typeof(DirectoryInfo))
            {
                if (selected == null)
                    return -1;
                int deep = 0;
                string tmp = selected.FullName;
                while (tmp != CurrentDirectoryInfo.CurrentDir)
                {
                    tmp = Path.GetDirectoryName(tmp);
                    deep++;
                }
                CurrentDirectoryInfo.UpdateDir(selected.FullName);
                await Task.Run(UpdateFileListAsync);
                UpdateBackDeep(deep);
                return 1;
            }
            return -1;
        }

        internal async Task BackAsync(object sender, EventArgs e)
        {
            UpdateBackDeep(-1);
            CurrentDirectoryInfo.BackDir();
            await UpdateFileListAsync();
        }
        async void CopyDirectory(DirectoryInfo sourceDir, string destinationPath)
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
                string targetFilePath = Path.Combine(newSourcePath, file.Name);
                if (File.Exists(targetFilePath))
                {
                    if (await ArletForExisted(targetFilePath, file.Name))
                    {
                        int num = 0;
                        while (File.Exists(string.Format("{0}{1}", targetFilePath, num)))
                        {
                            num++;
                        }
                        file.CopyTo(string.Format("{0}{1}", targetFilePath, num));
                        File.Delete(targetFilePath);
                        File.Move(string.Format("{0}{1}", targetFilePath, num), targetFilePath);
                    }
                }
            }

            foreach (DirectoryInfo directory in sourceDirTemp)
            {
                CopyDirectory(directory, newSourcePath);
            }

        }
        bool IsDirectoryContainDirectory(string dir1, string dir2)
        {
            while (dir2 != null)
            {
                if (dir2 == dir1) { return true; }
                DirectoryInfo tmp = Directory.GetParent(dir2);
                dir2 = (tmp != null) ? tmp.FullName : null;
            }
            return false;
        }  
        async Task<bool> ArletForExisted(string dir, string dirName)
        {
            bool result = false;
            if (File.Exists(dir))
            {
                result = await Shell.Current.DisplayAlert("Existed", "File "
                                                            + dirName
                                                            + "already exists in this directory\n"
                                                            + "Write new file or keep old file?",
                                                            "Write new", "Keep old");
            }
            return result;
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

        internal async Task DeleteModeAsync()
        {
            foreach (FileSystemInfoWithIcon f in CurrentFileList)
            {
                if (f.CheckBoxSelected)
                {
                    if (f.fileInfo.GetType() == typeof(FileInfo))
                    {
                        (f.fileInfo as FileInfo).Delete();
                    }
                    else if (f.fileInfo.GetType() == typeof(DirectoryInfo))
                    {
                        (f.fileInfo as DirectoryInfo).Delete(true);
                    }
                }
            }
            await UpdateFileListAsync();
            OperatedFileList.Clear();
        }
        internal async Task RenameModeAsync(string path, string newName)
        {
            if (Directory.Exists(path))
            {
                string newPath = Path.Combine(Directory.GetParent(path).FullName, newName);
                if (Directory.Exists(newPath))
                {
                    await Shell.Current.DisplayAlert("Duplicated", "Duplicate folder name, please choose another name", "OK");
                    return;
                }
                Directory.Move(path, newPath);
            }
            else if (File.Exists(path))
            {
                string newPath = Path.Combine(Directory.GetParent(path).FullName, newName) + Path.GetExtension(path);
                if (File.Exists(newPath))
                {
                    await Shell.Current.DisplayAlert("Duplicated", "Duplicate file name, please choose another name", "OK");
                    return;
                }

                File.Move(path, newPath);
            }
            await UpdateFileListAsync();
        }
        internal async Task PasteModeAsync()
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
                                if (File.Exists(targetFilePath))
                                {
                                    if (await ArletForExisted(targetFilePath, f.fileInfo.Name))
                                    {
                                        int num = 0;
                                        while (File.Exists(string.Format("{0}{1}", targetFilePath, num)))
                                        {
                                            num++;
                                        }
                                    (f.fileInfo as FileInfo).MoveTo(string.Format("{0}{1}", targetFilePath, num));
                                        File.Delete(targetFilePath);
                                        File.Move(string.Format("{0}{1}", targetFilePath, num), targetFilePath);
                                    }
                                }
                                else
                                {
                                    (f.fileInfo as FileInfo).MoveTo(targetFilePath);
                                }
                            }
                            else if (f.fileInfo.GetType() == typeof(DirectoryInfo))
                            {
                                string targetFilePath = Path.Combine(CurrentDirectoryInfo.CurrentDir, f.fileInfo.Name);
                                if (IsDirectoryContainDirectory(f.fileInfo.FullName, CurrentDirectoryInfo.CurrentDir))
                                {
                                    await Shell.Current.DisplayAlert("Error", f.fileInfo.Name + "\nCannot cut to itself", "OK");
                                    continue;
                                }
                                if (Directory.Exists(targetFilePath))
                                {
                                    await Shell.Current.DisplayAlert("Error", f.fileInfo.Name + "\nDirectory with same name already exists", "OK");
                                    continue;
                                }
                                (f.fileInfo as DirectoryInfo).MoveTo(targetFilePath);
                            }
                            break;
                        }
                    case FileSelectOption.Copy:
                        { 
                            if (f.fileInfo.GetType() == typeof(FileInfo))
                            {
                                string targetFilePath = Path.Combine(CurrentDirectoryInfo.CurrentDir, f.fileInfo.Name);
                                if (File.Exists(targetFilePath))
                                {
                                    if (await ArletForExisted(targetFilePath, f.fileInfo.Name))
                                    {
                                        int num = 0;
                                        while (File.Exists(string.Format("{0}{1}", targetFilePath, num)))
                                        {
                                            num++;
                                        }
                                    (f.fileInfo as FileInfo).CopyTo(string.Format("{0}{1}", targetFilePath, num));
                                        File.Delete(targetFilePath);
                                        File.Move(string.Format("{0}{1}", targetFilePath, num), targetFilePath);
                                    }
                                }
                                else
                                {
                                    (f.fileInfo as FileInfo).CopyTo(targetFilePath);
                                }

                            }
                            else if (f.fileInfo.GetType() == typeof(DirectoryInfo))
                            {
                                if (IsDirectoryContainDirectory(f.fileInfo.FullName, CurrentDirectoryInfo.CurrentDir))
                                {
                                    await Shell.Current.DisplayAlert("Error", f.fileInfo.Name + "\nCannot cut to itself", "OK");
                                    continue;
                                }
                                else
                                {
                                    CopyDirectory((f.fileInfo as DirectoryInfo), CurrentDirectoryInfo.CurrentDir);
                                }
                            }
                        break;
                        }
                }
            }
            await UpdateFileListAsync();
        }
        internal async Task<bool> NewFolderAsync(string name)
        {
            string path = Path.Combine(CurrentDirectoryInfo.CurrentDir, name);
            if (Directory.Exists(path))
            {
                await Shell.Current.DisplayAlert("Duplicated", "Duplicate folder name, please choose another name", "OK");
                return false; 
            }
            Directory.CreateDirectory(path);
            await UpdateFileListAsync();
            return true;
        }
        internal async Task SearchFileListAsync(string value)
        {
            IsReloading = true;
            DirectoryInfo dir = new DirectoryInfo(CurrentDirectoryInfo.CurrentDir);

            IEnumerable<System.IO.DirectoryInfo> directoryList = null;
            IEnumerable<System.IO.FileInfo> fileList = null;
            await Task.Run(async () =>
            {
                CurrentFileList.Clear();
                directoryList = dir.GetDirectories(string.Format("*{0}*", value), System.IO.SearchOption.AllDirectories);
                foreach (DirectoryInfo directoryInfo in directoryList)
                {
                    CurrentFileList.Add(new FileSystemInfoWithIcon(directoryInfo, "folder.png", 45));
                }
                fileList = dir.GetFiles(string.Format("*{0}*", value), System.IO.SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileList)
                {
                    await Task.Run(() => 
                    {
                        CurrentFileList.Add(new FileSystemInfoWithIcon(fileInfo, MaiIcon.GetIcon(fileInfo.Extension), 40));
                    });
                }
                if (IsSelectionMode)
                {
                    foreach (FileSystemInfoWithIcon f in CurrentFileList)
                    {
                        f.CheckBoxSelectVisible = true;
                    }
                }
                NumberOfCheked = 0;
            });
            IsReloading = false;
        }
        internal bool IsValidFileName(string name)
        {
            string invalidList = "|\\?*<\":>+[]/'";
            return (name.IndexOfAny(invalidList.ToCharArray()) == -1);
        }
    }
}
