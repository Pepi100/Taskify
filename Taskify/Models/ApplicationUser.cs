using Microsoft.AspNetCore.Identity;

namespace Taskify.Models
{
    public class ApplicationUser:IdentityUser
    {
        public virtual ICollection<UserProject>? UserProjects { get; set; }
    }
}
