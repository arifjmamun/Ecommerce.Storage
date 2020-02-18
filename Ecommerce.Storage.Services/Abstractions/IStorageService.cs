using Google.Apis.Drive.v3;

namespace Ecommerce.Storage.Services.Abstractions
{
  public interface IStorageService
  {
    DriveService GoogleDrive { get; set; }
    void InitializeCloudDriveService();
  }
}