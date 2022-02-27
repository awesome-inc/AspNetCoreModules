using Microsoft.EntityFrameworkCore;

namespace SampleApp.Data;

public class SampleAppContext : DbContext
{
    public SampleAppContext(DbContextOptions<SampleAppContext> options) : base(options)
    {
    }

    // See https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Blog)
            .WithMany(b => b.Posts)
            .HasForeignKey(p => p.BlogId);
    }
}
