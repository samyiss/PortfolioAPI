using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> projects { get; set; }
    public DbSet<Tag> tags { get; set; }
    public DbSet<Picture> pictures { get; set; }
    public DbSet<Author> authors { get; set; }
    public DbSet<Authors_Projects> tags_projects { get; set; }
    public DbSet<Authors_Projects> authors_projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>().HasKey(p => p.project_id);
        modelBuilder.Entity<Tag>().HasKey(p => p.tag_id);
        modelBuilder.Entity<Author>().HasKey(p => p.author_id);
        modelBuilder.Entity<Picture>().HasKey(p => p.picture_id);

        modelBuilder.Entity<Authors_Projects>().HasKey(bc => new { bc.project_id, bc.author_id });
        modelBuilder.Entity<Tags_Projects>().HasKey(bc => new { bc.project_id, bc.tag_id });

        modelBuilder.Entity<Authors_Projects>()
            .HasOne(bc => bc.authors)
            .WithMany(b => b.authors_projects)
            .HasForeignKey(bc => bc.author_id);

        modelBuilder.Entity<Authors_Projects>()
            .HasOne(bc => bc.projects)
            .WithMany(c => c.authors_projects)
            .HasForeignKey(bc => bc.project_id);

        modelBuilder.Entity<Tags_Projects>()
            .HasOne(bc => bc.tags)
            .WithMany(b => b.tags_projects)
            .HasForeignKey(bc => bc.tag_id);

        modelBuilder.Entity<Tags_Projects>()
            .HasOne(bc => bc.projects)
            .WithMany(c => c.Tags_projects)
            .HasForeignKey(bc => bc.project_id);

        base.OnModelCreating(modelBuilder);
    }

}