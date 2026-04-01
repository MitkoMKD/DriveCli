using DriveCli.Models;
using DriveCli.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveCli.Services
{
    public class SyncService: ISyncService
    {
        private readonly IGoogleDriveService _driveService;
        private readonly IFileSystemService _fileSystem;
        int maxConcurrency = Environment.ProcessorCount;
        private readonly ILogger<SyncService> _logger;
        public SyncService(IGoogleDriveService driveService, IFileSystemService fileSystem, ILogger<SyncService> logger)
        {
            _driveService = driveService;
            _fileSystem = fileSystem;
            _logger = logger;
        }
                
        public async Task<SyncResult> SyncWithProgressAsync(Spectre.Console.ProgressTask progressTask)
        {
            var files = await _driveService.GetFilesAsync();

            progressTask.MaxValue = files.Count;

            int success = 0;
            int failed = 0;

            var stopwatch = Stopwatch.StartNew();
            var semaphore = new SemaphoreSlim(maxConcurrency);

            var tasks = files.Select(async file =>
            {
                await semaphore.WaitAsync();

                try
                {
                    await _driveService.DownloadFileAsync(file.Id, file.Name, "Downloads");
                    Interlocked.Increment(ref success);
                }
                catch
                {
                    Interlocked.Increment(ref failed);
                    _logger.LogError($"Failed to download {file.Name}");
                }
                finally
                {
                    progressTask.Increment(1);
                    semaphore.Release();
                    _logger.LogInformation($"Processed {file.Name} - Success: {success}, Failed: {failed}");
                }
            });

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            return new SyncResult
            {
                Total = files.Count,
                Success = success,
                Failed = failed,
                ElapsedSeconds = stopwatch.Elapsed.TotalSeconds
            };
        }
    }
}
