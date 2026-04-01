using DriveCli.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveCli.Commands
{
    public class UploadCommand
    {
        private readonly IGoogleDriveService _drive;

        public UploadCommand(IGoogleDriveService drive)
        {
            _drive = drive;
        }

        public async Task ExecuteAsync(string localPath, string drivePath)
        {
            Console.WriteLine($"Uploading {localPath}...");
            await _drive.UploadFileAsync(localPath, drivePath);
            Console.WriteLine("Upload complete");
        }
    }
}
