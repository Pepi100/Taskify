using Taskify.Data;
using Microsoft.AspNetCore.Mvc;
using Taskify.Models;
using Task = Taskify.Models.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Taskify.Controllers
{
    [Authorize]

    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TasksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //se afiseaza lista tuturor taskurilor impreuna cu proiectul din care fac parte

        public IActionResult Index()
        {
       
            return View();
        }

        public IActionResult Show(int id)
        {
            var task = db.Tasks.Include("Comments.User").Where(tsk => tsk.Id == id).First();
            return View(task);
        }

        [HttpPost]

        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View(task);

        }

        [HttpPost]
        public IActionResult Edit(int id, Task requestTask)
        {
            Task task = db.Tasks.Find(id);

            if(DateTime.Compare(requestTask.StartDate, requestTask.EndDate) >= 0)
            {
                TempData["message"] = "Data de inceput este dupa data de final!";
                return Redirect("/Tasks/Edit/" + id);
            }

            if (ModelState.IsValid )
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
