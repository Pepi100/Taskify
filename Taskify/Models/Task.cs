using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskify.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please insert the task title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please insert the description of this task")]
        [StringLength(200, ErrorMessage = "The task description must have at most 200 characters")]
        public string Description { get; set; }
        public string Status { get; set; } /*Not Started, In Progress, Completed*/


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /*Persoana care creaza taskul*/
        public string? UserId { get; set; } ///punem string ca e hash

        public virtual ApplicationUser? User { get; set; }


        public int? ProjectId { get; set; }

        public virtual Project? Project { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Statuses { get; set; }
    }
}
