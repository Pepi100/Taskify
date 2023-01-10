using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Taskify.Models
{
    public class Comment
    {

        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please insert your comment")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? TaskId { get; set; }

        public virtual Task? Task { get; set; }

        /*Persoana care comenteaza*/
        public string? UserId { get; set; } ///punem string ca e hash

        public virtual ApplicationUser? User { get; set; }
        

    }
}
