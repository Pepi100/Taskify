using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taskify.Models;

namespace Taskify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage()
        {
            TempData["mail"] = "Thanks! The message has been sent!";
            return Redirect("/Home/Contact");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
       
        public IActionResult Error(int id)
        {

            ViewBag.Id = id;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}