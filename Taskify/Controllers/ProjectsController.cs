using Microsoft.AspNetCore.Mvc;

namespace Taskify.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
