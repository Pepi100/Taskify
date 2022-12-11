using Taskify.Data;
using Microsoft.AspNetCore.Mvc;
using Taskify.Models;
using Comment = Taskify.Models.Comment;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
namespace Taskify.Controllers
{
    public class CommentsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Edit(int id)
        {

            Comment comment = db.Comments.Where(comm=> comm.Id == id).First();
            var userid = _userManager.GetUserId(User);
            if (comment.UserId == userid || User.IsInRole("Admin"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
                return Redirect("/Tasks/Show/" + comment.TaskId);

            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            
            if (ModelState.IsValid)
            {
                comm.Content = requestComment.Content;
                comm.Date = DateTime.Now;
                
                TempData["message"] = "Comentariul a fost modificat";
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comm.TaskId);
            }
            else
            {
                return View(requestComment);
            }
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            var userid = _userManager.GetUserId(User);
            if (comm.UserId == userid)
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                TempData["message"] = "Comentariul a fost sters";
            }
            else
            {
                TempData["message"] = "Error! Nu ai acces";
                ViewBag.Message = TempData["message"];
            }
            return Redirect("/Tasks/Show/" + comm.TaskId);


        }



    }
}
