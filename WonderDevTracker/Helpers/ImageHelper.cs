﻿using WonderDevTracker.Models;

namespace WonderDevTracker.Helpers
{
    public static class ImageHelper
    {
        public static readonly string DefaultProfilePictureUrl = "/img/ProfilePlaceHolder.png";

        public static async Task<FileUpload> GetImageUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] data = ms.ToArray();
            if (ms.Length > 1 * 1024 * 1024) throw new Exception("The image is too large.");

            FileUpload imageUpload = new()
            {
                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType
            };
            return imageUpload;
        }
    }
}
