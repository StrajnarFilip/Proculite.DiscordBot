using Microsoft.AspNetCore.Mvc;
using Proculite.DiscordBot.Services;

namespace Proculite.DiscordBot.Controllers
{
    public class RoleAssignmentController : Controller
    {
        private readonly ILogger<RoleAssignmentController> _logger;
        private readonly RoleAssignmentService _roleAssignmentService;

        public RoleAssignmentController(
            ILogger<RoleAssignmentController> logger,
            RoleAssignmentService roleAssignmentService
        )
        {
            _logger = logger;
            this._roleAssignmentService = roleAssignmentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddRoleAssignment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessageByLink(
            [FromForm] string messageLink,
            [FromForm] string onlyOne
        )
        {
            _logger.LogInformation(onlyOne);
            await this._roleAssignmentService.AddRoleAssignmentMessageByMessageLink(
                messageLink,
                onlyOne == "on"
            );
            return RedirectToAction("Index");
        }
    }
}
