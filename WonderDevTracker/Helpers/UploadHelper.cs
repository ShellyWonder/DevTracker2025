using System.Text.RegularExpressions;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Models;

namespace WonderDevTracker.Helpers
{
    public static partial class UploadHelper
    {
        public static readonly string DefaultProfilePictureUrl = "/img/ProfilePlaceHolder.png";

        public static async Task<FileUpload> GetFileUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] data = ms.ToArray();
            if (ms.Length > BrowserFileHelper.MaxFileSize) throw new IOException("The selected file is too large.");

            FileUpload upload = new()
            {
                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType
            };
            return upload;
        }

        public static FileUpload GetFileUpload(string dataUrl)
        {
            GroupCollection matchGroups = DataUrlRegex().Match(dataUrl).Groups;

            if (matchGroups.ContainsKey("type") && matchGroups.ContainsKey("data"))
            {
                string contentType = matchGroups["type"].Value;
                string base64Data = matchGroups["data"].Value;
                byte[] data = Convert.FromBase64String(base64Data);
                if (data.Length<= BrowserFileHelper.MaxFileSize)
                {
                    FileUpload upload = new()
                    {
                        Id = Guid.NewGuid(),
                        Data = data,
                        Type = contentType
                    };
                    return upload; 
                }
                // data exists but is too large
                throw new IOException("The provided data URL is too large.");
            }
            // regex didn't provide "type" or "data" groups
            throw new IOException("The provided data URL is invalid.");

        }
        
        //@"data:(?<type>. +?);base64,(?<data>.+) builds formCompany.ImageUrl = $"data:{file.ContentType};base64,{base64String}";"
        //(?<type>. +?) ==> matches the content type (e.g., image/png)
        // capture group = data in (?<data>.+) ==> matches the actual base64-encoded data
        [GeneratedRegex(@"data:(?<type>.+?);base64,(?<data>.+)")]
        private static partial Regex DataUrlRegex();
    }
}
