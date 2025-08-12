using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;
using WonderDevTracker.Models;

namespace WonderDevTracker.Controllers
{
    [Route("api/uploads")]
    [ApiController]
    public class UploadsController(ApplicationDbContext _context) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60 * 24)]
        public async Task<IActionResult> GetImage(Guid id)
        {
            FileUpload? image = await _context.Uploads.FirstOrDefaultAsync(i => i.Id == id);
            if (image is null) return NotFound();

            //return byte array to the frontend for conversion into a src tag displaying on the page
            return File(image.Data!, image.Type!);
        }
    }
}
