using System;
using System.IO;
using System.Threading;
using Ecommerce.Storage.Services.Abstractions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Storage.Services.Implementations
{
  public class StorageService : IStorageService
  {
    private readonly IHostEnvironment _environment;

    private readonly string[] _scopes = { DriveService.Scope.Drive };
    private UserCredential _credential = null;
    private string _tokenPath => Path.Combine(_environment.ContentRootPath, "Secret", "token.json");
    private string _credentialPath => Path.Combine(_environment.ContentRootPath, "Secret", "credentials.json");

    public DriveService GoogleDrive { get; set; }

    public StorageService(IHostEnvironment environment)
    {
      _environment = environment;
      this.AuthorizeGoogleDrive();
    }

    private bool AuthorizeGoogleDrive()
    {
      using(var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read))
      {
        try
        {
          // The file token.json stores the user's access and refresh tokens, and is created
          // automatically when the authorization flow completes for the first time.
          _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            _scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(_tokenPath, true)
          ).Result;

          return _credential != null;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return false;
        }
      }
    }

    public void InitializeCloudDriveService()
    {
      if (GoogleDrive != null) return;
      GoogleDrive = new DriveService(
        new BaseClientService.Initializer
        {
          HttpClientInitializer = _credential,
            ApplicationName = "Ecommerce"
        }
      );
    }
  }
}