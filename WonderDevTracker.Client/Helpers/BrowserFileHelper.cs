using Microsoft.AspNetCore.Components.Forms;

namespace WonderDevTracker.Client.Helpers
{
    public static class BrowserFileHelper
    {
        public static readonly int MaxFileSize = 5 * 1024 * 1024; // 5 MB
        public static async Task<byte[]> GetFileDataAsync(IBrowserFile file)
        {
            if (file.Size < MaxFileSize)
            {
                try
                {    //store file as byte array
                    await using Stream fileStream = file.OpenReadStream(MaxFileSize);
                    await using MemoryStream ms = new();
                    await fileStream.CopyToAsync(ms);
                    return ms.ToArray();

                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }



            }

            throw new IOException("The file is invalid or too large.");

        }
    }
}
