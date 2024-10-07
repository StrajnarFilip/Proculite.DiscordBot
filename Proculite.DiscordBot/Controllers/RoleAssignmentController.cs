using Microsoft.AspNetCore.Mvc;

namespace Proculite.DiscordBot.Controllers
{
    public class RoleAssignmentController : Controller
    {
        private readonly ILogger<RoleAssignmentController> _logger;

        public RoleAssignmentController(ILogger<RoleAssignmentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
