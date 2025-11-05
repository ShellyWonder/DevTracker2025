using Microsoft.AspNetCore.Components.Forms;

namespace WonderDevTracker.Client.Models.Records
{
    public sealed record AttachmentUploadRequest(IBrowserFile File, string Description);
}
