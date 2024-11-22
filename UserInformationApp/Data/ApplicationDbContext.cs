using Microsoft.EntityFrameworkCore;
using UserInformationApp.Models;

namespace UserInformationApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Gender data
            modelBuilder.Entity<Gender>().HasData(
                new Gender { Id = 1, GenderName = "Male" },
                new Gender { Id = 2, GenderName = "Female" }
            );

            // Seed Nationality data
            modelBuilder.Entity<Nationality>().HasData(
                new Nationality { Id = 1, NationalityName = "American" },
                new Nationality { Id = 2, NationalityName = "Myanmar" }
            );

            // Configuring relationships between User and Gender, Nationality
            modelBuilder.Entity<User>()
                .HasOne(u => u.Gender) // Each User has one Gender
                .WithMany() // Gender can be referenced by many Users
                .HasForeignKey(u => u.GenderId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete of Gender if it is referenced by Users

            modelBuilder.Entity<User>()
                .HasOne(u => u.Nationality) // Each User has one Nationality
                .WithMany() // Nationality can be referenced by many Users
                .HasForeignKey(u => u.NationalityId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete of Nationality if it is referenced by Users

            base.OnModelCreating(modelBuilder);
        }
    }
}
