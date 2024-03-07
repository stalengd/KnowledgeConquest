using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeConquest.Server.Models
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<UserMapCell> UserMapCells { get; set; } = default!;
        public DbSet<Question> Questions { get; set; } = default!;
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; } = default!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserMapCell>()
                .HasKey(x => new { x.UserId, x.PositionX, x.PositionY });
            builder.Entity<QuestionAnswer>()
                .HasKey(x => new { x.QuestionId, x.Index });
        }
    }
}
