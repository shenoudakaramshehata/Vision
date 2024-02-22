using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vision.Models;

namespace Vision.Data
{
  
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Interest> Interests { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Education> Educations { get; set; }
        public virtual DbSet<LifeEvent> LifeEvents { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);            
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });

        }
    }
}