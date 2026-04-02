using Google.Apis.Drive.v3;

namespace DriveCli.Services.Interfaces
{
    public interface IAuthService
    {
        Task<DriveService> GetDriveServiceAsync();
    }
}
