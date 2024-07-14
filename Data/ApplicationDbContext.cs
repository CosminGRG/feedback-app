using FeedbackApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FeedbackApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<FeedbackModel> Feedbacks { get; set; }
    }
}
