using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Storage.Services.Abstractions
{
  public interface ICloudDriveService
  {
    Task<string> GetFolderIdByName(string folderName);
    Task<string> CreateShareableFolder(string folderName, string parentFolderName = null);
    Task<string> UploadAvatar(string userId, IFormFile file);
    Task<string> UploadProductFeaturedImage(string productId, IFormFile file);
    Task<List<string>> UploadProductImages(string productId, List<IFormFile> files);
    Task<string> SetPermission(string id);
  }
}