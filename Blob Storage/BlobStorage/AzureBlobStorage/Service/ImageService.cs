using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobStorage.Models;
using AzureBlobStorage.Service.Interface;

namespace AzureBlobStorage.Service
{
    public class ImageService : ImageInterface
    {

        private readonly BlobServiceClient _blobServiceClient;

        public ImageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        /* public async Task<string> UploadImage(Files file)
         {
             //creating instance for the container
             var container = _blobServiceClient.GetBlobContainerClient("images");

             //creating blob instance 
             var blob = container.GetBlobClient(file.Image.FileName);

             // Upload the image
             await blob.UploadAsync(file.Image.OpenReadStream());

             // Get the URL of the uploaded blob
             var blobUrl = blob.Uri.ToString();

             // Return the URL
             return blobUrl;
         }*/


        /// <summary>
        /// In this method the BlobHttpHeaders object to set the Content-Disposition header to "inline" and included it in the BlobUploadOptions.
        /// this will retrun a url which only display the image in the browser. without this option the url won't display the image in the browser instead it will download the image.
        /// above method will download the image when we use the link.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        
        public async Task<string> UploadImage(Files file)
        {
            // Creating instance for the container
            var container = _blobServiceClient.GetBlobContainerClient("images");

            // Creating blob instance 
            var blob = container.GetBlobClient(file.Image.FileName);

            // Set the Content-Disposition header to "inline"
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.Image.ContentType, ContentDisposition = "inline" };

            // Upload the image with specified headers
            await blob.UploadAsync(file.Image.OpenReadStream(), new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            // Get the URL of the uploaded blob
            var blobUrl = blob.Uri.ToString();

            // Return the URL
            return blobUrl;
        }


        public async Task<Stream> GetImage(string fileName)
        {
            //creating instance for the container
            var container = _blobServiceClient.GetBlobContainerClient("images");

            //blob instance
            var image = container.GetBlobClient(fileName);

            // fetch the image from the azure container
            var fetchImage = await image.DownloadAsync();

            // return's the content in the file
            return fetchImage.Value.Content;
        }
        

        
        public async Task<Stream> GetImageUsingUrl(string imageUri)
        {
            var blobClient = new BlobClient(new Uri(imageUri));

            // Download the image
            var response = await blobClient.DownloadAsync();

            // Get the image stream
            return response.Value.Content;
        }


        public async Task<string> Geturl(string fileName)
        {
            //creating instance for the container
            var container =  _blobServiceClient.GetBlobContainerClient("images");

            //blob instance
            var image = container.GetBlobClient(fileName);

            // return's the content in the file
            return image.Uri.ToString();
        }
    }
}
