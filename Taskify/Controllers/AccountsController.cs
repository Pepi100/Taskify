using Microsoft.AspNetCore.Mvc;

namespace Taskify.Controllers
{
    public class AccountsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
