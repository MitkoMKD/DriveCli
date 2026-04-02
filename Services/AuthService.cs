using DriveCli.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;

namespace DriveCli.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<IAuthService> _logger;

        public AuthService(ILogger<IAuthService> logger)
        {
            _logger = logger;
        }
        public async Task<DriveService> GetDriveServiceAsync()
        {
            try
            {
                string relativePath = Path.Combine("Config", "client_secret.json");
                string fullPath = Path.GetFullPath(relativePath);
                using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)
                );

                _logger.LogInformation("Google Drive authentication successful");

                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive CLI"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Google Drive authentication failed");
                throw;
            }

        }
    }
}
