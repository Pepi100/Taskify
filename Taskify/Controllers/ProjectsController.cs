using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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
            var userid = _userManager.GetUserId(User);
            var projects = db.Projects.Include("User").Where(proj => proj.UserId == userid);
            ViewBag.Projects = projects;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult IndexAdmin()
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
            var userid = _userManager.GetUserId(User);
            var project = db.Projects.Include("Tasks.User").Include("User").Where(proj => proj.Id == id).First();
            if (userid == project.UserId || User.IsInRole("Admin"))
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return RedirectToAction("Index");
            }
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

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Users(int id)
        {
            var userid = _userManager.GetUserId(User);
            var users_search = db.Users.Where(a => 1 == 0);
            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                // eliminam spatiile libere
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                users_search = db.Users.Where(usn => usn.UserName
                                                .Contains(search))
                                                .OrderBy(a => a.UserName);
            }

            var users = db.UserProjects.Include("User").Where(user => user.ProjectId == id);
            var project = db.Projects.Find(id);
            ViewBag.Users = users;
            ViewBag.Project = project;
            ViewBag.AllUsers = users_search;
            return View();
        }
        
        [HttpPost]
        public IActionResult Users(int id, [FromForm] UserProject requestUser)
        {
            var userid = _userManager.GetUserId(User);
            var project = db.Projects.Find(id);
            if (userid == project.UserId || User.IsInRole("Admin"))
            {
                UserProject userProject = new UserProject();
                userProject.ProjectId = id;
                userProject.UserId = requestUser.UserId;
                db.UserProjects.Add(userProject);
                db.SaveChanges();
                return Redirect("/Projects/Users/" + project.Id);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return RedirectToAction("Index");
            }
        }
    }
}
