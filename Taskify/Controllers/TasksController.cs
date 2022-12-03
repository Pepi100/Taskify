using Taskify.Data;
using Microsoft.AspNetCore.Mvc;
using Taskify.Models;
using Task = Taskify.Models.Task;

namespace Taskify.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;

        public TasksController(ApplicationDbContext context)
        {
            db = context;
        }


        //se afiseaza lista tuturor taskurilor impreuna cu proiectul din care fac parte

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id) 
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            TempData["message"] = "Taskul a fost sters";
            return Redirect("/Projects/Show/" + task.ProjectId);
            /*delete comms too*/
        }
    }
}
