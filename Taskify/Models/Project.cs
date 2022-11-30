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

        /*[Required]
        public virtual int OrganizerId { get; set; }*/

        public virtual ICollection<Task> Tasks { get; set; }



    }
}
