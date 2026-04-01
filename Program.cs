// See https://aka.ms/new-console-template for more information
using DriveCli.Commands;
using DriveCli.Services;
using DriveCli.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();

// Logging
services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

// Register all dependencies
services.AddSingleton<IAuthService, AuthService>();
services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
services.AddScoped<IFileSystemService, FileSystemService>();

// Register SyncService (concrete)
services.AddScoped<SyncService>();

// If you also need the interface for other services
services.AddScoped<ISyncService, SyncService>();

// Register commands
services.AddTransient<SyncCommand>();
services.AddTransient<SearchCommand>();
services.AddTransient<UploadCommand>();

var provider = services.BuildServiceProvider();

if (args.Length == 0)
{
    AnsiConsole.MarkupLine("[red]No command provided[/]");
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "sync":
        var syncCommand = provider.GetRequiredService<SyncCommand>();
        await syncCommand.ExecuteAsync();
        break;

    case "search":
        var searchCommand = provider.GetRequiredService<SearchCommand>();
        var query = args.Length > 1 ? args[1] : "";
        await searchCommand.ExecuteAsync(query);
        break;

    case "upload":
        if (args.Length < 3)
        {
            AnsiConsole.MarkupLine("[red]Usage: upload [localPath] [drivePath][/]");
            return;
        }

        var uploadCommand = provider.GetRequiredService<UploadCommand>();
        await uploadCommand.ExecuteAsync(args[1], args[2]);
        break;

    default:
        AnsiConsole.MarkupLine("[red]Unknown command[/]");
        break;
}