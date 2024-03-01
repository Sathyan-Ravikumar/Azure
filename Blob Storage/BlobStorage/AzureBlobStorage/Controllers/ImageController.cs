using Azure.Storage.Blobs;
using AzureBlobStorage.Models;
using AzureBlobStorage.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;

namespace AzureBlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageInterface _imageInterface;

        public ImageController(ImageInterface imageInterface)
        {
            _imageInterface = imageInterface;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Upload([FromForm] Files image)
        {
          var imageUrl =   await _imageInterface.UploadImage(image);
          return imageUrl;
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(string fileName)
        {
            var imageStream = await _imageInterface.GetImage(fileName);
            string fileType = "jpeg";
            if (fileName.Contains("png"))
            {
                fileType = "png";
            }
            if (fileName.Contains("jfif"))
            {
                fileType = "jfif";
            }
            if(fileName.Contains("svg")){
                fileType = "svg";
            }
            if (imageStream == null)
            {
                return NotFound("No image is on that name");
            }

            return File(imageStream, $"image/{fileType}"); // adjust the content type based on your image format
        }

        [HttpGet("Get the Image by its url")]

        public async Task<IActionResult> GetImageByUrl(string Url)
        {
            var image = await _imageInterface.GetImageUsingUrl(Url);
            string fileType = "jpeg";
            if (Url.Contains("png"))
            {
                fileType = "png";
            }
            if (Url.Contains("jfif"))
            {
                fileType = "jfif";
            }
            if (Url.Contains("svg"))
            {
                fileType = "svg";
            }
            if (image == null)
            {
                return NotFound("No image is on that name"); 
            }
            return File(image, $"image/{fileType}");
        }


        [HttpGet("Get Image URL")]

        public async Task<ActionResult<string>> GetUrl(string fileName)
        {
            var url = await _imageInterface.Geturl(fileName);
            return Ok(url);
        }

    }
}
