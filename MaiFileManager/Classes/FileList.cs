﻿using MaiFileManager.Services;
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
                if (await IsExistedFile(targetFilePath, file.Name))
                {
                    int num = 0;
                    while (File.Exists(string.Format("{0}{1}",targetFilePath,num)))
                    {
                        num++;
                    }
                    file.CopyTo(string.Format("{0}{1}", targetFilePath, num));
                    File.Delete(targetFilePath);
                    File.Move(string.Format("{0}{1}", targetFilePath, num), targetFilePath);
                }
                else
                {
                    file.CopyTo(targetFilePath);
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
        async Task<bool> IsExistedFile(string dir, string dirName)
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
            OperatedFileList.Clear();
        }
        internal void RenameMode(string path, string newName)
        {
            if (Directory.Exists(path))
            {
                string newPath = Path.Combine(Directory.GetParent(path).FullName, newName);
                if (Directory.Exists(newPath))
                {
                    Shell.Current.DisplayAlert("Duplicated", "Duplicate folder name, please choose another name", "OK");
                    return;
                }
                Directory.Move(path, newPath);
            }
            else if (File.Exists(path))
            {
                string newPath = Path.Combine(Directory.GetParent(path).FullName, newName) + Path.GetExtension(path);
                if (File.Exists(newPath))
                {
                    Shell.Current.DisplayAlert("Duplicated", "Duplicate file name, please choose another name", "OK");
                    return;
                }

                File.Move(path, newPath);
            }
            UpdateFileList();
        }

        internal async void PasteMode()
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
                                if (await IsExistedFile(targetFilePath, f.fileInfo.Name))
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
                                if (await IsExistedFile(targetFilePath, f.fileInfo.Name))
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
                                else
                                { 
                                    (f.fileInfo as FileInfo).CopyTo(targetFilePath);
                                }
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
