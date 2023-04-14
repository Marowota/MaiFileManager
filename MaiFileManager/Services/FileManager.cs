using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MaiFileManager.Services
{
    public partial class FileManager : INotifyPropertyChanged
    {
        string currentDir;
        public string CurrentDir 
        { 
            get => currentDir;
            set 
            {
                currentDir = value;
                OnPropertyChanged(nameof(CurrentDir));
                OnPropertyChanged(nameof(DirName));
            } 
        }
        public string DirName
        {
            get
            {
                return Path.GetFileName(CurrentDir);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public partial ObservableCollection<FileSystemInfo> GetListFile();
        public partial void UpdateDir(string newDir);
        public partial void BackDir();
    }
}
