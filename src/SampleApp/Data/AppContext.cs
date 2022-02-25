using Microsoft.EntityFrameworkCore;
using SampleApp.Api;

namespace SampleApp.Data;

public class AppContext : DbContext
{
    // See https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();
    
    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Blog)
            .WithMany(b => b.Posts)
            .HasForeignKey(p => p.BlogId);
    }
    
    public DbSet<SampleApp.Api.BlogDto> BlogDto { get; set; }
}
