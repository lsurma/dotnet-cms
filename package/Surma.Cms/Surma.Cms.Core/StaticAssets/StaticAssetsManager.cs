using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Surma.Cms.Core.Assets;
using System.Threading.Tasks;

namespace Surma.Cms.Core.StaticAssets;

public class StaticAssetsManager : IStaticAssetsManager
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }
    
    protected static volatile Task? _initializationTask;
    
    protected static readonly object InitializationLock = new object();
    
    protected static ConcurrentDictionary<string, StaticAssetData> _staticAssets = new ConcurrentDictionary<string, StaticAssetData>();

    public StaticAssetsManager(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
    }

    public async Task<StaticAssetData?> FindAssetDataAsync(StaticAssetFilter filter, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedOnceAsync();
        
        var cacheKey = GetCacheKey(filter);
        var cachedData = _staticAssets.GetValueOrDefault(cacheKey);
        
        if (cachedData is null)
        {
            var fetchedData = await FetchStaticAssetDataAsync(filter, true, cancellationToken);

            return fetchedData;
        }
        
        return cachedData;
    }

    protected virtual Task EnsureInitializedOnceAsync()
    {
        Console.WriteLine("EnsureInitializedOnceAsync");

        if (_initializationTask != null)
        {
            return _initializationTask;
        }
            
        lock (InitializationLock)
        {
            if (_initializationTask == null)
            {
                _initializationTask = InitializeAsync();
            }
        }
            
        return _initializationTask;
    }

    protected virtual async Task InitializeAsync()
    {
        Console.WriteLine("Initializing StaticAssetsManager");
        using var scope = ServiceScopeFactory.CreateScope();
        var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var staticAssets = await assetRepository.GetListAsync(q => q.IsStatic);
        var currentRevisions = staticAssets.Select(q => q.CurrentRevision).Where(q => q is not null).Cast<AssetRevision>().ToList();
        await assetRepository.LoadRevisionsContentAsync(currentRevisions);

        foreach (var asset in staticAssets)
        {
            var currentRevision = asset.CurrentRevision;
            
            if (currentRevision is null)
            {
                continue;
            }
            
            _staticAssets[GetCacheKey(currentRevision)] = MapFromAssetRevision(currentRevision);
        }
    }

    protected virtual async Task<StaticAssetData?> FetchStaticAssetDataAsync(
        StaticAssetFilter filter, 
        bool fallbackToCurrentRevision = true,
        CancellationToken cancellationToken = default
    )
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var revisionSelector = filter.GetRevisionSelectorOrUseCurrent();
        var asset = await assetRepository.FindAsync(
            q => q.Name == filter.Name, 
            revisionSelector, 
            cancellationToken
        );
        
        if (asset is null)
        {
            return null;
        }
        
        var revision = asset.FindRevision(revisionSelector);
        
        if(revision is null)
        {
            if (fallbackToCurrentRevision && asset.CurrentRevision is not null)
            {
                revision = asset.CurrentRevision;
                await assetRepository.LoadRevisionsContentAsync(new List<AssetRevision> { revision }, cancellationToken);
            }
            else
            {
                return null;
            }
        }
        
        return MapFromAssetRevision(revision);    
    }
    
    protected virtual string GetCacheKey(AssetRevision revision)
    {
        return $"{revision.Asset.Name}:{revision.GetName()}";
    }
    
    protected virtual string GetCacheKey(StaticAssetFilter filter)
    {
        return $"{filter.Name}:{filter.GetRevisionSelectorOrUseCurrent().ToString()}";
    }
    
    protected virtual StaticAssetData MapFromAssetRevision(AssetRevision revision)
    {
        return new StaticAssetData(
            revision.Asset.Name,
            revision.Content,
            revision.ContentLength,
            revision.ContentType,
            revision.GetLastModificationDate(),
            revision.GetName(),
            revision.IsCurrent
        );
    }
}