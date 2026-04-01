using DriveCli.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Linq.Expressions;

namespace DriveCli.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly IAuthService _authService;
        private readonly ILogger<IGoogleDriveService> _logger;


        public GoogleDriveService(IAuthService authService, ILogger<IGoogleDriveService> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        public async Task DownloadFileAsync(string fileId, string fileName, string path)
        {
            try
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
                _logger.LogInformation($"File '{fileName}' with ID '{fileId}' downloaded successfully to '{filePath}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file '{fileName}' with ID '{fileId}'");
                throw;
            }

        }

        public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFilesAsync()
        {
            try
            {
                var service = await _authService.GetDriveServiceAsync();

                var request = service.Files.List();
                request.PageSize = 1000;
                request.Fields = "files(id, name)";

                var result = await request.ExecuteAsync();
                _logger.LogInformation($"Fetched {result.Files.Count} files from Google Drive.");
                return result.Files;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching files from Google Drive");
                throw;
            }

        }

        public async Task<IList<Google.Apis.Drive.v3.Data.File>> SearchAsync(string query)
        {
            try
            {
                var service = await _authService.GetDriveServiceAsync();

                var request = service.Files.List();
                request.Q = $"name contains '{query}'";
                request.Fields = "files(id, name)";

                var result = await request.ExecuteAsync();
                _logger.LogInformation($"Search for '{query}' returned {result.Files.Count} files.");
                return result.Files;
            }
            catch (Exception)
            {
                _logger.LogError($"Error searching for files with query: {query}");
                throw;
            }

        }

        public async Task UploadFileAsync(string localPath, string drivePath)
        {
            try
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
                _logger.LogInformation($"File '{localPath}' uploaded successfully to '{drivePath}'");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error uploading file: {e.Message}");
                _logger.LogError(e, "Error uploading file");
                throw;
            }

        }
    }
}
