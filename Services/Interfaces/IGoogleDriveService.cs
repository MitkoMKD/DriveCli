namespace DriveCli.Services.Interfaces
{
    public interface IGoogleDriveService
    {
        Task<IList<Google.Apis.Drive.v3.Data.File>> GetFilesAsync();
        Task<IList<Google.Apis.Drive.v3.Data.File>> SearchAsync(string query);
        Task DownloadFileAsync(string fileId, string fileName, string path);
        Task UploadFileAsync(string localPath, string drivePath);
    }
}
