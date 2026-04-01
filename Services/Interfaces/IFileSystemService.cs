using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveCli.Services.Interfaces
{
    public interface IFileSystemService
    {
        bool Exists(string path);
        void CreateDirectory(string path);
    }
}
