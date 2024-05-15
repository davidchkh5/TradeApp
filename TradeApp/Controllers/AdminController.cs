using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradeApp.Controllers
{
    public class AdminController :BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]

        public ActionResult GetUsersWithRoles()
        {
            return Ok("Only admins can see this");
        }
    }
}
