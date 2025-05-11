using FileServ.Controllers.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileServ.Controllers
{
    [Route("api/contact_photos")]
    [ApiController]
    public class MainController : ControllerBase
    {
        //TODO async functions
        [HttpPost("uploadfile")] // api/main/uploadfile
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadHandler = new UploadHandler();
            var result = await uploadHandler.Upload(file);
            return Ok(result);
        }

        [HttpGet("retrievefile/{fileName}")]
        public async Task<IActionResult> RetrieveFile(string fileName)
        {
            var uploadHandler = new UploadHandler();

            try
            {
                var fileBytes = await uploadHandler.Retrieve(fileName);
                // Determine MIME type based on file extension
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var mimeType = fileExtension switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".txt" => "text/plain",
                    _ => "application/octet-stream" // Default for unknown types
                };
                return File(fileBytes, mimeType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("deletefile/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var uploadHandler = new UploadHandler();
            var result = await uploadHandler.Remove(fileName);

            if (result == "File not found.")
            {
                return NotFound(result);
            }
            else if (result.StartsWith("Error"))
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }
    }
}
