using Microsoft.AspNetCore.Mvc;

namespace Taskify.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
