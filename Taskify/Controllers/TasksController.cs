using Taskify.Data;
using Microsoft.AspNetCore.Mvc;

namespace Taskify.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TasksController(ApplicationDbContext context)
        {
            _db = context;
        }


        //se afiseaza lista tuturor taskurilor impreuna cu proiectul din care fac parte

        public IActionResult Index()
        {
            return View();
        }
    }
}
