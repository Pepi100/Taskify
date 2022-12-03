using Taskify.Data;
using Microsoft.AspNetCore.Mvc;
using Taskify.Models;
using Task = Taskify.Models.Task;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Show(int id)
        {
            var task = db.Tasks.Include("Comments").Where(tsk => tsk.Id == id).First();
            return View(task);
        }

        [HttpPost]

        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;    
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comment.TaskId);
            }
            else
            {
                Task task = db.Tasks.Include("Comments").Where(tsk => tsk.Id == comment.TaskId).First();
                return View(task);
            }
        }

        public IActionResult Edit(int id)
        {

            Task task = db.Tasks.Include("Comments")
                                        .Where(tsk => tsk.Id == id)
                                        .First();


            return View(task);

        }

        [HttpPost]
        public IActionResult Edit(int id, Task requestTask)
        {
            Task task = db.Tasks.Find(id);

            if (ModelState.IsValid)
            {
                task.Title = requestTask.Title;
                task.Description = requestTask.Description;
                task.Status = requestTask.Status;
                task.StartDate = requestTask.StartDate;
                task.EndDate = requestTask.EndDate;

                TempData["message"] = "Taskul a fost modificat";
                db.SaveChanges();
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            else
            {
                return View(requestTask);
            }
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
