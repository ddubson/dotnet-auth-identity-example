using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Produce.API.Controllers
{
    [Route("api/identity")]
    [Authorize]    
    public class IdentityController: ControllerBase
    {
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new {c.Type, c.Value});
        }
    }
}