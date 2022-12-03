﻿using System.ComponentModel.DataAnnotations;

namespace Taskify.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Description { get; set; }
        public string Status { get; set; } /*Not Started, In Progress, Completed*/
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Proiectul este obligatoriu")]
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
