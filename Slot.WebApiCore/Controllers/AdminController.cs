using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : ApiControllerBase
    {
        private readonly CachedSettings cachedSettings;
        private readonly ILogger<AdminController> logger;

        public AdminController(CachedSettings cachedSettings,
                               ILogger<AdminController> logger)
        {
            this.cachedSettings = cachedSettings;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ReLoad()
        {
            await Task.Delay(0);
            logger.LogInformation("Starting reload settings to cache..");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            cachedSettings.Load();
            stopwatch.Stop();
            logger.LogInformation("Done reload settings to cache.");
            return Ok(new
            {
                Elapsed = stopwatch.ElapsedMilliseconds
            });
        }
    }
}
