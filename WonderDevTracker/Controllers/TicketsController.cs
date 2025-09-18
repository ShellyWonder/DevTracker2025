using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;

using WonderDevTracker.Client.Services.Interfaces;

namespace WonderDevTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController(ITicketDTOService ticketService) : ControllerBase
    {
        // Check if the user is authenticated
        UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

    }
}
