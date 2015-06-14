using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using System.Threading;
using Google.Apis.Util.Store;
using System.Threading.Tasks;

namespace postsdi
{
    public class GoogleServiceInitializer
    {
        string clientSecretsFile;

        public GoogleServiceInitializer(string clientSecretsFile)
        {
            this.clientSecretsFile = clientSecretsFile;
        }

        async Task<UserCredential> UserCredential()
        {
            using (var filestream = new System.IO.FileStream(clientSecretsFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(filestream).Secrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("DriveCommandLineSample"));
                
                return credential;
            }
        }

        public async Task<DriveService> CreateDriveService()
        {
            var ds = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = await UserCredential(),
                ApplicationName = "Google Drive Static Site Deployment",
            });

            return ds;
        }
    }
}

