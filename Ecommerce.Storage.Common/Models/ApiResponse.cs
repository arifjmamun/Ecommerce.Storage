using System;

namespace Ecommerce.Storage.Common.Models
{
  public class ApiResponse<TResult>
  {
    public bool Success { get; set; }
    public TResult Result { get; set; }
    public string Message { get; set; }
  }

  public static class ApiResponseHelper
  {
    public static ApiResponse<TResult> CreateSuccessResponse<TResult>(this TResult result, string message = null)
    {
      return new ApiResponse<TResult> { Message = message, Result = result, Success = true };
    }

    public static ApiResponse<TResult> CreateErrorResponse<TResult>(this TResult result) where TResult : Exception
    {
      return new ApiResponse<TResult> { Message = result.Message, Result = null, Success = false };
    }
  }
}