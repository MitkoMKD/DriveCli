using DriveCli.Services.Interfaces;

namespace DriveCli.Services
{
    public class FileSystemService : IFileSystemService
    {
        public bool Exists(string path) => File.Exists(path);

        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
