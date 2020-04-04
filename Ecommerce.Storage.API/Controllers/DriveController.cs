using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Storage.Common.Models;
using Ecommerce.Storage.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Storage.API.Controllers
{
  [Route("api/drive")]
  [ApiController]
  public class DriveController : ControllerBase
  {
    private readonly ICloudDriveService _cloudDriveService;

    public DriveController(ICloudDriveService cloudDriveService)
    {
      _cloudDriveService = cloudDriveService;
    }

    /// <summary>
    /// Upload Profile avatar image
    /// </summary>
    /// <param name="userId"></param>
    [HttpPost("upload/avatar/{userId}")]
    public async Task<ActionResult<ApiResponse<string>>> UploadAvatar(string userId, IFormFile avatar)
    {
      try
      {
        var avatarUrl = await _cloudDriveService.UploadAvatar(userId, avatar);
        return avatarUrl.CreateSuccessResponse();
      }
      catch (Exception exception)
      {
        return BadRequest(exception.CreateErrorResponse());
      }
    }

    /// <summary>
    /// Upload products featured image
    /// </summary>
    /// <param name="productId"></param>
    [HttpPost("products/{productId}/upload/featured-image")]
    public async Task<ActionResult<ApiResponse<string>>> UploadProductFeaturedImage(string productId, IFormFile featureImage)
    {
      try
      {
        var file = await _cloudDriveService.UploadProductFeaturedImage(productId, featureImage);
        return file.CreateSuccessResponse();
      }
      catch (Exception exception)
      {
        return BadRequest(exception.CreateErrorResponse());
      }
    }

    /// <summary>
    /// Upload products images
    /// </summary>
    /// <param name="productId"></param>
    [HttpPost("products/{productId}/upload/images")]
    public async Task<ActionResult<ApiResponse<List<string>>>> UploadProductImages(string productId, List<IFormFile> images)
    {
      try
      {
        var files = await _cloudDriveService.UploadProductImages(productId, images);
        return files.CreateSuccessResponse();
      }
      catch (Exception exception)
      {
        return BadRequest(exception.CreateErrorResponse());
      }
    }
  }
}