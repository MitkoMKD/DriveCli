using DriveCli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveCli.Services.Interfaces
{
    public interface ISyncService
    {
        Task<SyncResult> SyncAsync();
        Task<SyncResult> SyncWithProgressAsync(Spectre.Console.ProgressTask progressTask);
    }
}
