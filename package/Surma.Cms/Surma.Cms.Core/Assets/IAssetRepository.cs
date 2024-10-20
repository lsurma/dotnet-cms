using System.Linq.Expressions;

namespace Surma.Cms.Core.Assets;

public interface IAssetRepository
{
    public Task<List<Asset>> GetListAsync
    (
        Expression<Func<Asset, bool>>? filter = null,
        CancellationToken cancellationToken = default
    );
    
    public Task<Asset?> FindAsync(
        Guid id,
        AssetRevisionSelector includeContentForRevision,
        CancellationToken cancellationToken = default
    );
    
    public Task<Asset?> FindAsync(
        Expression<Func<Asset, bool>> filter,
        AssetRevisionSelector includeContentForRevision,
        CancellationToken cancellationToken = default
    );
    
    public Task LoadRevisionsContentAsync
    (
        IList<AssetRevision> assetRevisionsToLoadContentFor,
        CancellationToken cancellationToken = default
    );
}