using DriveCli.Services;
using DriveCli.Services.Interfaces;
using FluentAssertions;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DriveCli.UnitTests
{
    public class GoogleDriveServiceTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<IGoogleDriveService>> _mockLogger;
        private readonly GoogleDriveService _service;

        public GoogleDriveServiceTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<IGoogleDriveService>>();
            _service = new GoogleDriveService(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetFilesAsync_ShouldThrowException_WhenAuthServiceFails()
        {
            // Arrange
            _mockAuthService.Setup(a => a.GetDriveServiceAsync())
                .ThrowsAsync(new InvalidOperationException("Auth failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetFilesAsync());

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error fetching files")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UploadFileAsync_ShouldThrowFileNotFoundException_WhenLocalFileDoesNotExist()
        {
            // Arrange
            var invalidPath = @"C:\nonexistent\file.txt";
            var drivePath = "/drive/path";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _service.UploadFileAsync(invalidPath, drivePath));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error uploading file")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}