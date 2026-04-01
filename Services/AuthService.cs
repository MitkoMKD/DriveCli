using DriveCli.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveCli.Services
{
    public class AuthService : IAuthService
    {
        public AuthService() { }
        public async Task<DriveService> GetDriveServiceAsync()
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

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive CLI"
            });
        }
    }
}
