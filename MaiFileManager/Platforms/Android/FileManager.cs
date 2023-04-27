using Android;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Net;
using AndroidX.Core.App;
using MaiFileManager;
using static Microsoft.Maui.ApplicationModel.Platform;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

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
        public FileManager(int type)
        {
            switch (type)
            {
                case 0:
                    currentDir = global::Android.OS.Environment.ExternalStorageDirectory.Path;
                    break;
                case 1:
                    currentDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                    break;
            }
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

        public partial bool GetPerm()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                if (!Android.OS.Environment.IsExternalStorageManager)
                {
                    Android.Content.Intent intent = new Android.Content.Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                    Android.Net.Uri uri = Android.Net.Uri.FromParts("package", AppInfo.PackageName, null);
                    intent.SetData(uri);
                    Platform.CurrentActivity.StartActivity(intent);
                }
                return (Android.OS.Environment.IsExternalStorageManager);
            }
            else
            {
                return false;
            }
        }

        public partial void UpdateDir(string newDir)
        {
            CurrentDir = newDir;
        }

        public partial void BackDir()
        {
            CurrentDir = Directory.GetParent(CurrentDir).FullName;
        }


        //public static partial void OpenWith(string path)
        //{

        //    try
        //    {
        //        Intent intent = new Intent(Intent.ActionView);
        //        Java.IO.File file = new Java.IO.File(path);
        //        string MIMO = Java.Nio.FileNio.Files.ProbeContentType(file.ToPath());
        //        intent.SetDataAndType(Android.Net.Uri.FromFile(file), MIMO);
        //        Platform.CurrentActivity.StartActivity(intent);
        //    }
        //    catch (ActivityNotFoundException e)
        //    {
        //        // no Activity to handle this kind of files
        //    }

        //}
    }
}
