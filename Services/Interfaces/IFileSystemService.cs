namespace DriveCli.Services.Interfaces
{
    public interface IFileSystemService
    {
        bool Exists(string path);
        void CreateDirectory(string path);
    }
}
