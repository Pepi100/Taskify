using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
    }
}
