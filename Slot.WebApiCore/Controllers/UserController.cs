using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slot.Core.Services.Abstractions;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(IUserService userService,
                              IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate(string op, string token)
        {
            var ipAddr = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await userService.Authenticate(op, token, ipAddr);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFunPlayKey(string op)   
        {
            return Ok(await userService.GetFunplayKey(op));
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance(string token, string game, string platform)
        {
            return Ok(await userService.GetBalance(token, game, GetPlatformType(platform)));
        }
    }
}
