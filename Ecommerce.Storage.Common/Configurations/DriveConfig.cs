namespace Ecommerce.Storage.Common.Configurations
{
  public class DriveConfig : IDriveConfig
  {
    public string ImageUrlTemplate { get; set; }
  }

  public interface IDriveConfig
  {
    string ImageUrlTemplate { get; set; }
  }
}