using DriveCli.Services;
using DriveCli.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Spectre.Console;

namespace DriveCli.UnitTests
{
    public class SyncServiceTest
    {
        [Fact]
        public async Task SyncAsync_AllFilesDownloaded_ReturnsCorrectStats()
        {
            // Arrange
            var files = new List<Google.Apis.Drive.v3.Data.File>
            {
                new Google.Apis.Drive.v3.Data.File { Id = "1", Name = "file1.txt" },
                new Google.Apis.Drive.v3.Data.File { Id = "2", Name = "file2.txt" }
            };

            var driveMock = new Mock<IGoogleDriveService>();
            driveMock.Setup(x => x.GetFilesAsync()).ReturnsAsync(files);
            driveMock.Setup(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            var fileSystemMock = new Mock<IFileSystemService>();

            var service = new SyncService(
                driveMock.Object,
                fileSystemMock.Object,
                new NullLogger<SyncService>()
            );

            var progressTask = new ProgressTask(1, "Syncing files", 2, false);

            // Act
            var result = await service.SyncWithProgressAsync(progressTask);

            // Assert
            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.Success);
            Assert.Equal(0, result.Failed);
        }

        [Fact]
        public async Task SyncAsync_WhenDownloadFails_CountsFailures()
        {
            // Arrange
            var files = new List<Google.Apis.Drive.v3.Data.File>
    {
        new Google.Apis.Drive.v3.Data.File { Id = "1", Name = "file1.txt" },
        new Google.Apis.Drive.v3.Data.File { Id = "2", Name = "file2.txt" }
    };

            var driveMock = new Mock<IGoogleDriveService>();
            driveMock.Setup(x => x.GetFilesAsync()).ReturnsAsync(files);

            driveMock.Setup(x => x.DownloadFileAsync("1", It.IsAny<string>(), It.IsAny<string>()))
                     .ThrowsAsync(new Exception("fail"));

            driveMock.Setup(x => x.DownloadFileAsync("2", It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            var fileSystemMock = new Mock<IFileSystemService>();

            var service = new SyncService(
                driveMock.Object,
                fileSystemMock.Object,
                new NullLogger<SyncService>()
            );

            var progressTask = new ProgressTask(1, "Syncing files", 2, false);

            // Act
            var result = await service.SyncWithProgressAsync(progressTask);

            // Assert
            Assert.Equal(2, result.Total);
            Assert.Equal(1, result.Success);
            Assert.Equal(1, result.Failed);
        }

        [Fact]
        public async Task SyncAsync_CallsDownloadForEachFile()
        {
            // Arrange
            var files = new List<Google.Apis.Drive.v3.Data.File>
    {
        new Google.Apis.Drive.v3.Data.File { Id = "1", Name = "file1.txt" },
        new Google.Apis.Drive.v3.Data.File { Id = "2", Name = "file2.txt" }
    };

            var driveMock = new Mock<IGoogleDriveService>();
            driveMock.Setup(x => x.GetFilesAsync()).ReturnsAsync(files);
            driveMock.Setup(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

            var fileSystemMock = new Mock<IFileSystemService>();

            var service = new SyncService(
                driveMock.Object,
                fileSystemMock.Object,
                new NullLogger<SyncService>()
            );

            var progressTask = new ProgressTask(1, "Syncing files", 2, false);

            // Act
            var result = await service.SyncWithProgressAsync(progressTask);

            // Assert
            driveMock.Verify(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}