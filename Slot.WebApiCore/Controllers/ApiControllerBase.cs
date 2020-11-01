using Microsoft.AspNetCore.Mvc;
using Slot.Model;

namespace Slot.WebApiCore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ApiControllerBase : ControllerBase
    {
        protected PlatformType GetPlatformType(string platform)
        {
            if (string.IsNullOrWhiteSpace(platform)) {
                return PlatformType.Web;
            }
            var x = platform.ToLower();
            if (x == "mobile")
            {
                return PlatformType.Mobile;
            }
            else if (x == "mini")
            {
                return PlatformType.Mini;
            }
            else
            {
                return PlatformType.Web;
            }
        }
    }
}
