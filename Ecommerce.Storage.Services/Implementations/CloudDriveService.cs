using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Storage.Common.Configurations;
using Ecommerce.Storage.Services.Abstractions;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Storage.Services.Implementations
{
  public class CloudDriveService : ICloudDriveService
  {
    private readonly IStorageService _storageService;
    private readonly IDriveConfig _driveConfig;
    public CloudDriveService(IStorageService storageService, IDriveConfig driveConfig)
    {
      _storageService = storageService;
      _driveConfig = driveConfig;
      _storageService.InitializeCloudDriveService();
    }

    public async Task<string> GetFolderIdByName(string folderName)
    {
      try
      {
        string pageToken = null;
        var request = this._storageService.GoogleDrive.Files.List();
        request.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{folderName}'";
        request.PageSize = 10;
        request.Fields = "nextPageToken, files(id, name)";
        request.PageToken = pageToken;
        var response = await request.ExecuteAsync();
        return response.Files?[0] != null ? response.Files[0].Id : null;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return null;
      }
    }

    public async Task<string> CreateShareableFolder(string folderName, string parentFolderName = null)
    {
      try
      {
        var folder = new File { Name = folderName, MimeType = "application/vnd.google-apps.folder" };
        if (!string.IsNullOrWhiteSpace(parentFolderName))
        {
          var parentFolderId = await GetFolderIdByName(parentFolderName);
          if (parentFolderId == null)
          {
            parentFolderId = await CreateShareableFolder(parentFolderName);
          }
          folder.Parents = new List<string> { parentFolderId };
        }
        var request = _storageService.GoogleDrive.Files.Create(folder);
        request.Fields = "id";
        var response = await request.ExecuteAsync();
        await this.SetPermission(response.Id);
        return response.Id;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return ex.Message;
      }
    }

    public async Task<File> UploadProfile(IFormFile file)
    {
      try
      {
        var folderId = await GetFolderIdByName("public");
        if (folderId == null)
        {
          folderId = await CreateShareableFolder("public");
        }

        var stream = file.OpenReadStream();
        var fileMetadata = new File
        {
          Name = $"{DateTime.Now.ToLongTimeString()}_{file.FileName}",
          MimeType = file.ContentType,
          Parents = new List<string> { folderId }
        };
        var request = _storageService.GoogleDrive.Files.Create(
          fileMetadata,
          stream,
          file.ContentType
        );
        var response = await request.UploadAsync();
        if (response.Status == Google.Apis.Upload.UploadStatus.Completed)
        {
          await SetPermission(request.ResponseBody.Id);
          return request.ResponseBody;
        }
        return null;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return null;
      }
    }

    public async Task<string> SetPermission(string id)
    {
      try
      {
        var permission = new Permission { Type = "anyone", Role = "writer" };
        var request = _storageService.GoogleDrive.Permissions.Create(permission, id);
        var response = await request.ExecuteAsync();
        return response.Id;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return null;
      }
    }

    public async Task<string> UploadProductFeaturedImage(string productId, IFormFile file)
    {
      try
      {
        var featuredImageUrl = await UploadProductImage(productId, file, "featured_images");
        return featuredImageUrl;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return null;
      }
    }

    public async Task<List<string>> UploadProductImages(string productId, List<IFormFile> files)
    {
      try
      {
        var imageUrls = new List<string>();
        for (int i = 0; i < files.Count; i++)
        {
          var file = files[i];
          var imageUrl = await UploadProductImage(productId, file, "images");
          imageUrls.Add(imageUrl);
        }
        return imageUrls;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return null;
      }
    }

    private async Task<string> UploadProductImage(string productId, IFormFile file, string type)
    {
      var folderId = await GetFolderIdByName(type);
      if (folderId == null)
      {
        folderId = await CreateShareableFolder(type, "products");
      }

      var stream = file.OpenReadStream();
      var fileMetadata = new File
      {
        Name = type == "images" ? $"{productId}_{DateTime.Now.ToLongTimeString()}_{file.FileName}" : $"{productId}_{file.FileName}",
        MimeType = file.ContentType,
        Parents = new List<string> { folderId }
      };
      var request = _storageService.GoogleDrive.Files.Create(
        fileMetadata,
        stream,
        file.ContentType
      );
      var response = await request.UploadAsync();
      if (response.Status == Google.Apis.Upload.UploadStatus.Completed)
      {
        await SetPermission(request.ResponseBody.Id);
        var imageUrl = _driveConfig.ImageUrlTemplate.Replace("{id}", request.ResponseBody.Id);
        return imageUrl;
      }
      return null;
    }
  }
}