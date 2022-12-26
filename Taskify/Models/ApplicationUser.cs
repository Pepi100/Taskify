using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskify.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required(ErrorMessage = "Please insert your First Name"), MaxLength(100)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please insert your Last Name"), MaxLength(100)]
        public string LastName { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public virtual ICollection<UserProject>? UserProjects { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        public virtual ICollection<Project>? Projects{ get; set; }

        public virtual ICollection<Task>? Tasks{ get; set; }

        public virtual ICollection<Comment>? Comments{ get; set; }
    }
}
