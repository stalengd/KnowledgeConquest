using Microsoft.EntityFrameworkCore;

namespace KnowledgeConquest.UtilityCli.Models
{
    public class AppDbContext : DbContext
    {
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

            builder.Entity<QuestionAnswer>()
                .HasKey(x => new { x.QuestionId, x.Index });
        }
    }
}
