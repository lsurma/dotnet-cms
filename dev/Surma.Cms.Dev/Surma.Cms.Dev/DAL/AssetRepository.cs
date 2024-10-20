using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Surma.Cms.Core.Assets;
using Surma.CMS.Dev.DAL;
using Surma.CMS.Dev.Scrutor;

namespace Surma.Cms.Dev.DAL;

public class AssetRepository : IAssetRepository, ITransient
{
    public AppDbContext DbContext { get; }
    
    public AssetRepository(
        AppDbContext dbContext
    )
    {
        DbContext = dbContext;

    }

    public Task<List<Asset>> GetListAsync(Expression<Func<Asset, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var queryable = GetBaseQueryWithIncludes();
        
        if (filter is not null)
        {
            queryable = queryable.Where(filter);
        }
        
        return queryable
            .Select(ProjectToAssetWithoutContent())
            .ToListAsync(cancellationToken);
    }
    
    public Task<Asset?> FindAsync(Guid id, bool includeContent = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<Asset?> FindAsync(Guid id, AssetRevisionSelector includeContentForRevision, CancellationToken cancellationToken = default)
    {
        return FindAsync(q => q.Id == id, includeContentForRevision, cancellationToken); 
    }
    
    public async Task<Asset?> FindAsync(
        Expression<Func<Asset, bool>> filter, 
        AssetRevisionSelector includeContentForRevision, 
        CancellationToken cancellationToken = default
    )
    {
        var queryable = GetBaseQueryWithIncludes();
        var asset = await queryable
            .Where(filter)
            .Select(ProjectToAssetWithoutContent())
            .FirstOrDefaultAsync(cancellationToken);

        if (asset is null)
        {
            return null;
        }
        
        var revisionToLoadedContentFor = asset.FindRevision(includeContentForRevision);
        
        if (revisionToLoadedContentFor is not null)
        {
            await LoadRevisionsContentAsync(new List<AssetRevision> { revisionToLoadedContentFor }, cancellationToken);
        }
        
        return asset;
    }
    

    public async Task LoadRevisionsContentAsync(IList<AssetRevision> assetRevisionsToLoadContentFor, CancellationToken cancellationToken = default)
    {
        var ids = assetRevisionsToLoadContentFor.Select(q => q.Id).ToList();
        var revisions = await DbContext.AssetRevisions
            .Where(q => ids.Contains(q.Id))
            .Select(q => new
            {
                q.Id,
                q.Content
            })
            .ToListAsync(cancellationToken: cancellationToken);

        foreach (var revision in revisions)
        {
            var assetRevision = assetRevisionsToLoadContentFor.First(q => q.Id == revision.Id);
            assetRevision.Content = revision.Content;
            assetRevision.IsContentLoaded = true;
        }
    }

    protected static Expression<Func<Asset, Asset>> ProjectToAssetWithoutContent()
    {
        return asset => new Asset
        {
            Id = asset.Id,
            DisplayName = asset.DisplayName,
            Name = asset.Name,
            Type = asset.Type,
            CurrentRevisionId = asset.CurrentRevisionId,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt,
            CurrentRevision = asset.CurrentRevision == null ? null : ProjectToRevisionWithoutContent(asset.CurrentRevision),
            Revisions = asset.Revisions.Select(x => ProjectToRevisionWithoutContent(x)).ToList()
        };
    }
    
    protected static AssetRevision ProjectToRevisionWithoutContent(AssetRevision revision)
    {
        return new AssetRevision
        {
            Id = revision.Id,
            Asset = revision.Asset,
            AssetId = revision.AssetId,
            Number = revision.Number,
            Description = revision.Description,
            PreviewKey = revision.PreviewKey,
            ContentType = revision.ContentType,
            ContentLength = revision.ContentLength,
            CreatedAt = revision.CreatedAt,
            UpdatedAt = revision.UpdatedAt
        };
    }
    
    protected IQueryable<Asset> GetBaseQueryWithIncludes()
    {
        return DbContext.Assets
            .Include(q => q.CurrentRevision)
                .ThenInclude(q => q.Asset)
            .Include(q => q.Revisions)
                .ThenInclude(q => q.Asset);
        
    }
}