using System.Security.Cryptography;
using System.Text;

namespace PhoneBook.Helpers
{
    public static class GenericHelpers
    {
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static bool IsValidImageFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            var lowerCaseFileName = fileName.ToLower();
            return lowerCaseFileName.EndsWith(".jpg") || lowerCaseFileName.EndsWith(".jpeg") || lowerCaseFileName.EndsWith(".png");
        }
        public static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

    }
}