using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    public class UploadsController(ApplicationDbContext _context) : ControllerBase
    {
        [SwaggerIgnore]
        [HttpGet("{id:guid}")]
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60 * 24)]
        public async Task<IActionResult> GetImage(Guid id)
        {
            FileUpload? image = await _context.Uploads.FirstOrDefaultAsync(i => i.Id == id);
            //prevents this endpoints from collecting Attachment image
            if (image is null || await _context.Attachments.AnyAsync(a => a.UploadId ==id)) return NotFound();

            //return byte array to the frontend for conversion into a src tag displaying on the page
            return File(image.Data!, image.Type!);
        }

        //Current use: TicketAttachments.ToDTO() uses this endpoint to get the attachment URL
        [SwaggerIgnore]
        [HttpGet("/api/attachments/{uploadId:guid}")]
        [Authorize]
        public async Task<IActionResult> DownloadAttachment([FromRoute] Guid uploadId)
        {
            UserInfo userInfo = UserInfoHelper.GetUserInfo(User)!;
            TicketAttachment? attachment = await _context.Attachments
                .Include(a => a.Upload)
                .FirstOrDefaultAsync(a => a.UploadId == uploadId
                                                   && a.Ticket!.Project!.CompanyId == userInfo.CompanyId);
            if (attachment is null) return NotFound();
            //Prevents the file being opened in the browser
            Response.Headers.TryAdd("Content-Disposition", $"attachment, filename={attachment.FileName}");
            return File(attachment.Upload!.Data!, attachment.Upload.Type!, attachment.FileName);

        }
    }
}
