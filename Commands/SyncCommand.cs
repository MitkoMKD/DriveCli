using DriveCli.Services;
using Spectre.Console;

namespace DriveCli.Commands
{
    public class SyncCommand
    {
        private readonly SyncService _syncService;

        public SyncCommand(SyncService syncService)
        {
            _syncService = syncService;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                AnsiConsole.MarkupLine("[yellow]Starting synchronization...[/]");

                var result = await AnsiConsole.Progress()
                    .StartAsync(async ctx =>
                    {
                        var progressTask = ctx.AddTask("[green]Downloading files...[/]");

                        return await _syncService.SyncWithProgressAsync(progressTask);
                    });

                AnsiConsole.MarkupLine("\n[green]✔ Sync completed successfully![/]");

                AnsiConsole.Write(new Rule("[blue]Statistics[/]").Centered());

                AnsiConsole.MarkupLine($"Total: {result.Total}");
                AnsiConsole.MarkupLine($"Downloaded: {result.Success}");
                AnsiConsole.MarkupLine($"Failed: {result.Failed}");
                AnsiConsole.MarkupLine($"Time: {result.ElapsedSeconds:F2}s");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            }
        }
    }
}
