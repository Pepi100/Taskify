using Taskify.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Taskify.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Models.Task> Tasks { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<UserProject> UserProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<UserProject>()
            .HasKey(ab => new {
                ab.Id,
                ab.UserId,
                ab.ProjectId
            });
            // definire relatii cu modelele Bookmark si Article (FK)
            modelBuilder.Entity<UserProject>()
            .HasOne(ab => ab.User)
            .WithMany(ab => ab.UserProjects)
            .HasForeignKey(ab => ab.UserId);

            modelBuilder.Entity<UserProject>()
            .HasOne(ab => ab.Project)
            .WithMany(ab => ab.UserProjects)
            .HasForeignKey(ab => ab.ProjectId);
        }
        
    }
}
