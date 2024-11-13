using lithuanian_language_learning_tool.Models;
using Microsoft.EntityFrameworkCore;

namespace lithuanian_language_learning_tool.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<PracticeSession> PracticeSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Achievements)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PracticeSessions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            // Configure LessonProgress as JSON if using a compatible database, or handle serialization in the application layer
        }

    }
}
