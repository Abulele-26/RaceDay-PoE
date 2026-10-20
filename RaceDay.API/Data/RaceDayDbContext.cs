using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using Route = RaceDay.API.Models.Route;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Weather> Weathers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure unique email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // One-to-One relationship between Enrollment and Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrollment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrollmentId);

            // Stop cascading deletes from Users to Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Stop cascading deletes from Categories to Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}