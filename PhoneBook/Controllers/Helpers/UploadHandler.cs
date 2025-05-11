namespace FileServ.Controllers.Helpers
{
    public class UploadHandler
    {
        /// <summary>
        /// Uploads the file to the server.
        /// </summary>
        /// <param name="file"></param>
        /// <returns> 
        /// success message or error message
        /// </returns>
        public async Task<string> Upload(IFormFile file)
        {
            // Validate the file extension
            List<string> allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension)){
                return $"Extension not allowed ({string.Join(',',allowedExtensions)})";
            }

            //validate the file size
            if (file.Length > 5 * 1024 * 1024) // 5 MB limit atm
            {
                return "File size exceeds the limit of 5 MB.";
            }

            // Name change GUID
            string fileName = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(),"Uploads");
            
            //using could be put instead of dispose and close
            FileStream stream1 = new FileStream(Path.Combine(path,fileName), FileMode.Create);
            file.CopyTo(stream1);
            stream1.Dispose();
            stream1.Close();

            return fileName;
        }
        /// <summary>
        /// Retrieves the file from the server.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<byte[]> Retrieve(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The requested file does not exist.", fileName);
            }

            return await File.ReadAllBytesAsync(path);
        }
        /// <summary>
        /// Removes the file from the server.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> Remove(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

            if (!File.Exists(path))
            {
                return "File not found.";
            }
            try
            {
                File.Delete(path);
                return "File successfully deleted.";
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return $"Error deleting file: {ex.Message}";
            }
        }
    }
}
