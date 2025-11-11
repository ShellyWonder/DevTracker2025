using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Models;

namespace WonderDevTracker.Helpers
{
    public static class UploadHelper
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
    }
}
