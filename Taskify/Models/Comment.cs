using System.ComponentModel.DataAnnotations;


namespace Taskify.Models
{
    public class Comment
    {

        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? TaskId { get; set; }

        public virtual Task? Task { get; set; }

        /*Persoana care comenteaza*/
        public string? UserId { get; set; } ///punem string ca e hash

        public virtual ApplicationUser? User { get; set; }
        

    }
}
