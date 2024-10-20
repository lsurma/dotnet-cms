using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Surma.Cms.Core.Assets;

namespace Surma.Cms.Core.StaticAssets;

public interface IStaticAssetsManager
{
    Task<StaticAssetData?> FindAssetDataAsync(
        StaticAssetFilter filter,
        CancellationToken cancellationToken = default
    );
}