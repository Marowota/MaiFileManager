using Android;
using Android.Content.PM;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiFileManager.Services
{
    public partial class FileManager : INotifyPropertyChanged
    {
        public FileManager(string dir)
        {
            currentDir = dir;
        }
        public FileManager() :
        this(global::Android.OS.Environment.ExternalStorageDirectory.Path)
        {
        }
        public partial ObservableCollection<FileSystemInfo> GetListFile()
        {
            ObservableCollection<FileSystemInfo> fileList = new ObservableCollection<FileSystemInfo>();
            fileList.Clear();
            if (currentDir == null)
            {
                return fileList;
            }
            if (!Directory.Exists(currentDir))
            {
                return fileList;
            }
            string[] files = Directory.GetFiles(currentDir);
            string[] folders = Directory.GetDirectories(currentDir);
            foreach (string folder in folders)
            {
                fileList.Add(new DirectoryInfo(folder));
            }
            foreach (string file in files)
            {
                fileList.Add(new FileInfo(file));
            }
            return fileList;
        }

        public partial void UpdateDir(string newDir)
        {
            CurrentDir = newDir;
        }

        public partial void BackDir()
        {
            CurrentDir = Directory.GetParent(CurrentDir).FullName;
        }
    }
}
