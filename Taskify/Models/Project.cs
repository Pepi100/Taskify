using System.ComponentModel.DataAnnotations;

namespace Taskify.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

         
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }



        /*Persoana care creaza proiectul*/
        public string? UserId { get; set; } ///punem string ca e hash

        public virtual ApplicationUser? User { get; set; }


        public virtual ICollection<Task>? Tasks { get; set; }
        public virtual ICollection<UserProject>? UserProjects { get; set; }

        /*Echipa?*/


    }
}
