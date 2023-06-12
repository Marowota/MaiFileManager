using CommunityToolkit.Maui.Core.Primitives;
using MaiFileManager.Services;
using Microsoft.Maui.Dispatching;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        public enum FileSortMode
        {
            NameAZ,
            NameZA,
            SizeSL,
            SizeLS,
            TypeAZ,
            TypeZA,
            DateNO,
            DateON
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
        public FileSortMode SortMode = (FileSortMode)Preferences.Default.Get("Sort_by", 0);
        System.Timers.Timer delayTime = new System.Timers.Timer(500);
        public ObservableCollection<FileSystemInfoWithIcon> OperatedFileListView { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>(); 
        public ObservableCollection<FileSystemInfoWithIcon> OperatedCompletedListView { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>(); 
        public ObservableCollection<FileSystemInfoWithIcon> OperatedErrorListView { get; set; } = new ObservableCollection<FileSystemInfoWithIcon>();

        private double operatedPercent = 0;
        private string operatedStatusString = "";
        public Page NavigatedPage = null;
        public double OperatedPercent
        {
            get
            {
                return operatedPercent;
            }
            set
            {
                operatedPercent = value;
                operatedStatusString = string.Format("{0:0} %, {1} / {2}",
                                                     operatedPercent * 100,
                                                     OperatedFileList.Count - OperatedFileListView.Count,
                                                     OperatedFileList.Count);
                OnPropertyChanged(nameof(OperatedPercent));
                OnPropertyChanged(nameof(OperatedStatusString));
            }
        }
        public string OperatedStatusString
        {
            get
            {
                return operatedStatusString;
            }
        }
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
            InitTimer();
        }
        public FileList(int type)
        {
            CurrentDirectoryInfo = new FileManager(type);
            InitTimer();
        }
        public FileList(string path)
        {
            CurrentDirectoryInfo = new FileManager(path);
            InitTimer();
        }

        void InitTimer()
        {
            delayTime.Interval = 500;
            delayTime.Elapsed += DelayTime_Elapsed;
            delayTime.Start();
        }

        private void DelayTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
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
        internal int SortFileComparisonFolderToFile(FileSystemInfo x, FileSystemInfo y)
        {
            return (x.GetType() == y.GetType() ? 0 : (x.GetType() == typeof(DirectoryInfo) ? -1 : 1));
        }
        internal int SortFileComparisonNameAZ(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        internal int SortFileComparisonNameZA(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            return string.Compare(y.Name, x.Name, StringComparison.OrdinalIgnoreCase);
        }

        internal int SortFileComparisonSizeSL(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            long sizeX = 0, sizeY = 0;
            if (x.GetType() == typeof(FileInfo))
            {
                sizeX = (x as FileInfo).Length;
            }
            if (y.GetType() == typeof(FileInfo))
            {
                sizeY = (y as FileInfo).Length;
            }
            return (sizeX < sizeY ? -1 : (sizeX > sizeY ? 1 : 0));
        }
        internal int SortFileComparisonSizeLS(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            long sizeX = 0, sizeY = 0;
            if (x.GetType() == typeof(FileInfo))
            {
                sizeX = (x as FileInfo).Length;
            }
            if (y.GetType() == typeof(FileInfo))
            {
                sizeY = (y as FileInfo).Length;
            }
            return (sizeX > sizeY ? -1 : (sizeX < sizeY ? 1 : 0));
        }
        internal int SortFileComparisonTypeAZ(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            return string.Compare(x.Extension, y.Extension, StringComparison.OrdinalIgnoreCase);
        }
        internal int SortFileComparisonTypeZA(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            return string.Compare(y.Extension, x.Extension, StringComparison.OrdinalIgnoreCase);
        }
        internal int SortFileComparisonDateNO(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            return DateTime.Compare(y.LastWriteTime, x.LastWriteTime);
        }
        internal int SortFileComparisonDateON(FileSystemInfo x, FileSystemInfo y)
        {
            int tmp = SortFileComparisonFolderToFile(x, y);
            if (tmp != 0) return tmp;

            return DateTime.Compare(x.LastWriteTime, y.LastWriteTime);
        }
        internal List<FileSystemInfo> SortFileMode(List<FileSystemInfo> fsi)
        {
            Comparison<FileSystemInfo> compare = new Comparison<FileSystemInfo>(SortFileComparisonNameAZ);

            switch (SortMode)
            {
                case FileSortMode.NameAZ:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonNameAZ);
                    break;
                case FileSortMode.NameZA:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonNameZA);
                    break;
                case FileSortMode.SizeSL:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonSizeSL);
                    break;
                case FileSortMode.SizeLS:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonSizeLS);
                    break;
                case FileSortMode.TypeAZ:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonTypeAZ);
                    break;
                case FileSortMode.TypeZA:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonTypeZA);
                    break;
                case FileSortMode.DateNO:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonDateNO);
                    break;
                case FileSortMode.DateON:
                    compare = new Comparison<FileSystemInfo>(SortFileComparisonDateON);
                    break;
            }
            fsi.Sort(compare);
            return fsi;
        }
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
                foreach (FileSystemInfo info in SortFileMode(CurrentDirectoryInfo.GetListFile().ToList()))
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
                Page tmp;
                if (NavigatedPage == null)
                {
                    tmp = Shell.Current.CurrentPage;
                }
                else
                {
                    tmp = NavigatedPage;
                }
                await tmp.Dispatcher.DispatchAsync(async () => {
                        result = await tmp.DisplayAlert("Existed", "File "
                                                                    + dirName
                                                                    + "already exists in this directory\n"
                                                                    + "Write new file or keep old file?",
                                                                    "Write new", "Keep old");
                    });

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

        internal async Task<int> DeleteModeAsync()
        {
            OperatedFileList.Clear();
            OperatedFileListView.Clear();
            OperatedCompletedListView.Clear();
            OperatedErrorListView.Clear();
            int noFIle = 1;
            foreach (FileSystemInfoWithIcon f in CurrentFileList)
            {
                if (f.CheckBoxSelected)
                {
                    noFIle = 0;
                    OperatedFileList.Add(f);
                    OperatedFileListView.Add(f);
                }
            }
            OperatedPercent = 0;
            int tmpInit = OperatedFileListView.Count;
            int tmpDone = 0;
            if (noFIle == 1) return 0;
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
                    tmpDone++;
                    OperatedFileListView.Remove(f);
                    OperatedCompletedListView.Add(f);
                    OperatedPercent = (double)tmpDone / tmpInit;
                }
            }
            await UpdateFileListAsync();
            OperatedFileList.Clear();
            return 1;
        }
        internal async Task RenameModeAsync(string path, string newName)
        {
            if (Directory.Exists(path))
            {
                string newPath = Path.Combine(Directory.GetParent(path).FullName, newName);
                if (Directory.Exists(newPath))
                {
                    Page tmp;
                    if (NavigatedPage == null)
                    {
                        tmp = Shell.Current.CurrentPage;
                    }
                    else
                    {
                        tmp = NavigatedPage;
                    }
                    //maybe dont need
                    await tmp.Dispatcher.DispatchAsync(async () => {
                        await tmp.DisplayAlert("Duplicated", "Duplicate folder name, please choose another name", "OK");
                    });
                    return;
                }
                Directory.Move(path, newPath);
            }
            else if (File.Exists(path))
            {
                string newPath = Path.Combine(Directory.GetParent(path).FullName, newName) + Path.GetExtension(path);
                if (File.Exists(newPath))
                {
                    Page tmp;
                    if (NavigatedPage == null)
                    {
                        tmp = Shell.Current.CurrentPage;
                    }
                    else
                    {
                        tmp = NavigatedPage;
                    }
                    //maybe dont need
                    await tmp.Dispatcher.DispatchAsync(async () => { await tmp.DisplayAlert("Duplicated", "Duplicate file name, please choose another name", "OK"); });
                    return;
                }

                File.Move(path, newPath);
            }
            await UpdateFileListAsync();
        }
        internal async Task PasteModeAsync()
        {
            OperatedFileListView.Clear();
            OperatedCompletedListView.Clear();
            OperatedErrorListView.Clear();
            foreach (FileSystemInfoWithIcon f in OperatedFileList)
            {
                OperatedFileListView.Add(f);
            }
            OperatedPercent = 0;
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
                                    Page tmp;
                                    if (NavigatedPage == null)
                                    {
                                        tmp = Shell.Current.CurrentPage;
                                    }
                                    else
                                    {
                                        tmp = NavigatedPage;
                                    }
                                    await tmp.Dispatcher.DispatchAsync(async () => { await tmp.DisplayAlert("Error", f.fileInfo.Name + "\nCannot cut to itself", "OK"); });
                                    OperatedFileListView.Remove(f);
                                    OperatedErrorListView.Add(f);
                                    OperatedPercent = (double)(OperatedFileList.Count - OperatedFileListView.Count) / OperatedFileList.Count;
                                    continue;
                                }
                                if (Directory.Exists(targetFilePath))
                                {
                                    Page tmp;
                                    if (NavigatedPage == null)
                                    {
                                        tmp = Shell.Current.CurrentPage;
                                    }
                                    else
                                    {
                                        tmp = NavigatedPage;
                                    }
                                    await tmp.Dispatcher.DispatchAsync(async () => { await tmp.DisplayAlert("Error", f.fileInfo.Name + "\nDirectory with same name already exists", "OK"); });
                                    OperatedFileListView.Remove(f);
                                    OperatedErrorListView.Add(f);
                                    OperatedPercent = (double)(OperatedFileList.Count - OperatedFileListView.Count) / OperatedFileList.Count;
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
                                    Page tmp;
                                    if (NavigatedPage == null)
                                    {
                                        tmp = Shell.Current.CurrentPage;
                                    }
                                    else
                                    {
                                        tmp = NavigatedPage;
                                    }
                                    await tmp.Dispatcher.DispatchAsync(async () => { await tmp.DisplayAlert("Error", f.fileInfo.Name + "\nCannot copy to itself", "OK"); });
                                    OperatedFileListView.Remove(f);
                                    OperatedErrorListView.Add(f);
                                    OperatedPercent = (double)(OperatedFileList.Count - OperatedFileListView.Count) / OperatedFileList.Count;
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
                OperatedFileListView.Remove(f);
                OperatedCompletedListView.Add(f);
                OperatedPercent = (double)(OperatedFileList.Count - OperatedFileListView.Count) / OperatedFileList.Count;
            }
            await UpdateFileListAsync();
        }
        internal async Task<bool> NewFolderAsync(string name)
        {
            if (!IsValidFileName(name))
            {
                await Shell.Current.DisplayAlert("Invalid", "Invalid folder name, please choose another name", "OK");
                return false;
            }
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
                directoryList = dir.GetDirectories("**", System.IO.SearchOption.AllDirectories)
                                   .Where(dirInfo 
                                    => dirInfo.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
                foreach (DirectoryInfo directoryInfo in directoryList)
                {
                    CurrentFileList.Add(new FileSystemInfoWithIcon(directoryInfo, "folder.png", 45));
                }
                fileList = dir.GetFiles("**", System.IO.SearchOption.AllDirectories)
                              .Where(fileInfo
                              => fileInfo.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
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
