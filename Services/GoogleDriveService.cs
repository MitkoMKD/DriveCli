using DriveCli.Services.Interfaces;
using Polly;
using Polly.Retry;

namespace DriveCli.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly IAuthService _authService;

        public GoogleDriveService(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task DownloadFileAsync(string fileId, string fileName, string path)
        {
            var service = await _authService.GetDriveServiceAsync();

            Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, fileName);

            AsyncRetryPolicy retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    3,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} for {fileName} due to {exception.Message}");
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                var request = service.Files.Get(fileId);

                await request.DownloadAsync(stream);
            });
        }

        public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFilesAsync()
        {
            var service = await _authService.GetDriveServiceAsync();

            var request = service.Files.List();
            request.PageSize = 1000;
            request.Fields = "files(id, name)";

            var result = await request.ExecuteAsync();
            return result.Files;
        }

        public async Task<IList<Google.Apis.Drive.v3.Data.File>> SearchAsync(string query)
        {
            var service = await _authService.GetDriveServiceAsync();

            var request = service.Files.List();
            request.Q = $"name contains '{query}'";
            request.Fields = "files(id, name)";

            var result = await request.ExecuteAsync();
            return result.Files;
        }

        public async Task UploadFileAsync(string localPath, string drivePath)
        {
            var service = await _authService.GetDriveServiceAsync();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(localPath)
            };

            if (!File.Exists(localPath))
            {
                throw new FileNotFoundException("File not found");
            }

            using var stream = new FileStream(localPath, FileMode.Open);

            var request = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id";

            await request.UploadAsync();
        }
    }
}
