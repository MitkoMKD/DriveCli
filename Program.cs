// See https://aka.ms/new-console-template for more information
using DriveCli.Commands;
using DriveCli.Services;
using Spectre.Console;

Console.WriteLine("Hello, World!");

var authService = new AuthService();
var driveService = new GoogleDriveService(authService);
var fileSystemService = new FileSystemService();
var syncService = new SyncService(driveService, fileSystemService);

if (args.Length == 0)
{
    AnsiConsole.MarkupLine("[red]No command provided[/]");
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "sync":
        await new SyncCommand(syncService).ExecuteAsync();
        break;

    case "search":
        var query = args.Length > 1 ? args[1] : "";
        await new SearchCommand(driveService, fileSystemService).ExecuteAsync(query);
        break;

    case "upload":
        if (args.Length < 3)
        {
            AnsiConsole.MarkupLine("[red]Usage: upload [localPath] [drivePath][/]");
            return;
        }
        await new UploadCommand(driveService).ExecuteAsync(args[1], args[2]);
        break;

    default:
        AnsiConsole.MarkupLine("[red]Unknown command[/]");
        break;
}