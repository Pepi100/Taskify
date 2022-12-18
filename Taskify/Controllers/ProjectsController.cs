using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        }

        [HttpPost]
        public async Task<IActionResult> NewAsync(Project project)
        {
            var user_id = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                project.UserId = user_id;   
                UserProject project_data = new UserProject();
                project_data.UserId = user_id;
                project_data.ProjectId = project.Id;
                project_data.Project = project;
                
                db.UserProjects.Add(project_data);
                await db.SaveChangesAsync();
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
            ///de verificat ca userul in sesiune sa fie in cadrul proiectului
            var users_search = db.Users.Where(a => 1 == 0);
            var users = db.UserProjects.Include("User").Where(user => user.ProjectId == id);
            List<string> users_ids = users.Select(c => c.UserId).ToList();
            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                users_search = db.Users.Where(usn => usn.UserName.Contains(search) && 
                                                    !users_ids.Contains(usn.Id))
                                        .OrderBy(a => a.UserName);/*
                users_search = db.Users.Where(usn => usn.UserName.Contains(search))
                                        .OrderBy(a => a.UserName);*/

            }

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
