using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Models;
using Task = Taskify.Models.Task;

namespace Taskify.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProjectsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Index()
        {
            var projects = db.Projects.Include("User");
            ViewBag.Projects = projects;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }


        public IActionResult New()
        {
            Project project = new Project();
            return View(project);
            /*de adaugat id user organiser*/
        }

        [HttpPost]
        public IActionResult New(Project project)
        {
            project.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                TempData["message"] = "The project wad added";
                return RedirectToAction("Index");
            }
            else
            {
                return View(project);
            }
        }
        [Authorize(Roles = "User,Editor,Admin")]

        public IActionResult Show(int id)
        {
            var project = db.Projects.Include("Tasks").Include("User").Where(proj => proj.Id == id).First();
            return View(project);
        }

        [HttpPost]

        public IActionResult Show([FromForm] Task task)
        {
            task.UserId = _userManager.GetUserId(User);


            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return Redirect("/Projects/Show/" + task.ProjectId);   
            }
            else
            {
                Project project = db.Projects.Include("Tasks").Include("User").Where(proj => proj.Id == task.ProjectId).First();
                return View(project);
            }
        }

        public IActionResult Edit(int id)
        {

            Project project = db.Projects.Include("Tasks")
                                        .Where(proj => proj.Id == id)
                                        .First();


            return View(project);

        }

        [HttpPost]
        public IActionResult Edit(int id, Project requestProject)
        {
            Project project = db.Projects.Find(id);

            if (ModelState.IsValid)
            {
                project.Title = requestProject.Title;
                project.Description = requestProject.Description;
                TempData["message"] = "Proiectul a fost modificat";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestProject);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            TempData["message"] = "Project has been deleted.";
            return RedirectToAction("Index");
        }

    }
}
