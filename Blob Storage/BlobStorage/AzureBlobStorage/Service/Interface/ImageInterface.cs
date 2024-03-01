using AzureBlobStorage.Models;

namespace AzureBlobStorage.Service.Interface
{
    public interface ImageInterface
    {
        Task<string> UploadImage(Files file);
        Task<Stream> GetImage(string fileName);
        Task<Stream> GetImageUsingUrl(string imageUri);
        Task<string> Geturl(string fileName);
    }
}
