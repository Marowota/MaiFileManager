using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiFileManager.Classes
{
    public class DirData
    {
        private string dir;
        private string name;
        private string lastModify;

        public string Dir { get => dir; set => dir = value; }
        public string Name { get => name; set => name = value; }
        public string LastModify { get => lastModify; set => lastModify = value; }
    }
}
