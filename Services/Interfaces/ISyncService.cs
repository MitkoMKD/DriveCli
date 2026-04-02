using DriveCli.Models;

namespace DriveCli.Services.Interfaces
{
    public interface ISyncService
    {
        Task<SyncResult> SyncWithProgressAsync(Spectre.Console.ProgressTask progressTask);
    }
}
