using DriveCli.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveCli.Commands
{
    public class SearchCommand
    {
        private readonly IGoogleDriveService _drive;
        private readonly IFileSystemService _fs;

        public SearchCommand(IGoogleDriveService drive, IFileSystemService fs)
        {
            _drive = drive;
            _fs = fs;
        }

        public async Task ExecuteAsync(string query)
        {
            var files = await _drive.SearchAsync(query);

            foreach (var file in files)
            {
                var exists = _fs.Exists($"Downloads/{file.Name}");
                var status = exists ? "[Downloaded]" : "[Not Downloaded]";

                Console.WriteLine($"{file.Name} - {status}");
            }
        }
    }
}
