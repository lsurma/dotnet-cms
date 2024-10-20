using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Surma.Cms.Core.Assets;
using Surma.Cms.Core.DynamicPages;

namespace Surma.Cms.Dev.DAL;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder
            .HasOne(a => a.CurrentRevision)
            .WithOne()
            .HasForeignKey<Asset>(a => a.CurrentRevisionId);

        builder
            .HasMany(a => a.Revisions)
            .WithOne(r => r.Asset)
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder
            .HasOne<Asset>()
            .WithMany()          
            .HasForeignKey(o => o.ContentAssetId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}