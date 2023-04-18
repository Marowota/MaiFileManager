using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MaiFileManager.Classes
{
    public class FileSystemInfoWithIcon
    {
        public FileSystemInfo fileInfo { get; set; }
        public string iconPath { get; set; }
        public int iconSize { get; set; }
        public string fileInfoSize { get; set; }
        public string[] fileSizeMeasure { get; set; } = { "B", "K", "M", "G", "T" };
        public string lastModified { get; set; }
        public FileSystemInfoWithIcon(FileSystemInfo fileData, string iconPathData, int iconSizeData)
        {
            fileInfo = fileData;
            iconPath = iconPathData;
            iconSize = iconSizeData;
            ConvertFileInfoSize();
            ConvertFileLastModified();
            
        }
        public void ConvertFileInfoSize()
        {

            if (fileInfo.GetType() == typeof(FileInfo))
            {
                double tmp = (fileInfo as FileInfo).Length;
                int i = 0;
                while (tmp > 1024)
                {
                    i++;
                    tmp /= 1024;
                    if (i == 4) break;
                }
                fileInfoSize = string.Format(" {0:0.#} {1}", tmp, fileSizeMeasure[i]);
            }
            else
            {
                fileInfoSize = "";
            }
        }

        public void ConvertFileLastModified()
        {
            lastModified = fileInfo.LastWriteTime.ToString("G", CultureInfo.GetCultureInfo("vi-VN"));
        }
    }
}
