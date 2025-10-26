using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WonderDevTracker.Client;
using WonderDevTracker.Client.Helpers;

namespace WonderDevTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketCommentsController : ControllerBase
    {
        private UserInfo  UserInfo  => UserInfoHelper.GetUserInfo(User)!;
    }
}
