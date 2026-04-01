using DriveCli.Models;
using DriveCli.Services.Interfaces;
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

        public SyncService(IGoogleDriveService driveService, IFileSystemService fileSystem)
        {
            _driveService = driveService;
            _fileSystem = fileSystem;
        }

        public async Task<SyncResult> SyncAsync()
        {
            var files = await _driveService.GetFilesAsync();

            int success = 0;
            int failed = 0;

            var stopwatch = Stopwatch.StartNew();
            var semaphore = new SemaphoreSlim(5);

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
                }
                finally
                {
                    semaphore.Release();
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

        public async Task<SyncResult> SyncWithProgressAsync(Spectre.Console.ProgressTask progressTask)
        {
            var files = await _driveService.GetFilesAsync();

            progressTask.MaxValue = files.Count;

            int success = 0;
            int failed = 0;

            var stopwatch = Stopwatch.StartNew();
            var semaphore = new SemaphoreSlim(5);

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
                }
                finally
                {
                    progressTask.Increment(1);
                    semaphore.Release();
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
