using Taskify.Data;
using Microsoft.AspNetCore.Mvc;
using Taskify.Models;
using Task = Taskify.Models.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace Taskify.Controllers
{
   
    [Authorize(Roles = "User,Admin")]
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

        [NonAction]
        public bool CheckUser(int? proj_id)
        {
            if(proj_id == null)
                return false;
            var userid = _userManager.GetUserId(User);
            var users = db.UserProjects.Where(userpr => userpr.ProjectId == proj_id).Select(c => c.UserId);
            List<string?> user_ids = users.ToList();
            if(user_ids.Contains(userid))
                return true;
            return false;
        }
        public IActionResult Show(int id)
        {
            
            var task = db.Tasks.Include("Comments.User").Include("User").Include("Project")
                                .Where(tsk => tsk.Id == id).First();
            ViewBag.Users = db.UserProjects.Include("User")
                .Where(c => c.ProjectId == task.ProjectId);
            if (CheckUser(task.ProjectId) || User.IsInRole("Admin"))
            {
                return View(task);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }
       
        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            Task task = db.Tasks.Include("Comments").Where(tsk => tsk.Id == comment.TaskId).First();
            if (CheckUser(task.ProjectId) || User.IsInRole("Admin"))
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
                    return View(task);
                }
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }
        [HttpPost]
        public IActionResult AddUser([FromForm] int TaskId, [FromForm] string userId)
        {
            var userid = _userManager.GetUserId(User);
            Task taskaux = db.Tasks.Include("Project").Where(a => a.Id == TaskId).First();
            if (userid == taskaux.Project.UserId || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    if (db.Tasks
                        .Where(task => task.Id == TaskId && task.UserId == userId)
                        .Count() > 0)
                    {
                        TempData["message"] = "User already has this task";
                        TempData["messageType"] = "alert-danger";
                    }
                    else
                    {
                        Task task = db.Tasks.Find(TaskId);
                        if (task != null)
                        {
                            task.UserId = userId;
                            db.SaveChanges();

                            TempData["message"] = "User added to task";
                            TempData["messageType"] = "alert-success";
                        }
                        else
                        {
                            TempData["message"] = "Database error!";
                            TempData["messageType"] = "alert-danger";
                        }
                    }

                }
                else
                {
                    TempData["message"] = "Try again!";
                    TempData["messageType"] = "alert-danger";
                }

                return Redirect("/Tasks/Show/" + TaskId);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }
        public IActionResult Edit(int id)
        {

            Task task = db.Tasks.Include("Comments")
                                        .Where(tsk => tsk.Id == id)
                                        .First();
            task.Statuses = GetAllStatuses();
            
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.CurrentStatus = task.Status;
            return View(task);

        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllStatuses()
        {
            var statuses = new List<SelectListItem>();
            string[] all_statuses = { "not started", "in progress", "completed" };
            foreach (var status in all_statuses)
            {
                string aux;
                if (status.Length == 1)
                {
                    aux = status.ToUpper();
                }
                else
                {
                    aux = char.ToUpper(status[0]) + status[1..];
                }
                statuses.Add(new SelectListItem
                {
                    Value = status.ToLower(),
                    Text = aux
                });
    
            }
            return statuses;
        }

        [HttpPost]
        public IActionResult Edit(int id, Task requestTask, [FromForm] string newStatus)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                TempData["message"] = "Database error!";
                return View(requestTask);

            }
            else
            {
                if (DateTime.Compare(requestTask.StartDate, requestTask.EndDate) >= 0)
                {
                    TempData["message"] = "Data de inceput este dupa data de final!";
                    return Redirect("/Tasks/Edit/" + id);
                }

                if (ModelState.IsValid)
                {
                    task.Title = requestTask.Title;
                    task.Description = requestTask.Description;
                    task.Status = newStatus;
                    task.StartDate = requestTask.StartDate;
                    task.EndDate = requestTask.EndDate;

                    TempData["message"] = "Taskul a fost modificat";
                    db.SaveChanges();
                    return Redirect("/Projects/Show/" + task.ProjectId);
                }
                else
                {
                    ViewBag.CurrentStatus = newStatus;
                    requestTask.Statuses = GetAllStatuses();
                    return View(requestTask);
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int id) 
        {
            var userid = _userManager.GetUserId(User);
            Task task = db.Tasks.Include("Comments").Include("Project").Where(tsk => tsk.Id == id).First();
            if (userid == task.Project.UserId || User.IsInRole("Admin"))
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
                TempData["message"] = "Taskul a fost sters";
                return Redirect("/Projects/Show/" + task.ProjectId);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Projects/Index");
            }
        }
    }
}
