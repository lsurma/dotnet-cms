using Microsoft.EntityFrameworkCore;
using Surma.Cms.Core.Assets;
using Surma.Cms.Core.DynamicPages;
using Surma.CMS.Dev.CMS;

namespace Surma.CMS.Dev.DAL;

public partial class AppDbContext : DbContext
{
    public DbSet<Page> Pages { get; set; }

    public DbSet<Asset> Assets { get; set; }

    public DbSet<AssetRevision> AssetRevisions { get; set; }
    
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
