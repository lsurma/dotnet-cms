using Microsoft.AspNetCore.Http;
using Surma.Cms.Core.Assets;

namespace Surma.Cms.Core.StaticAssets;

public record StaticAssetFilter
(
    string Name,
    AssetRevisionSelector? RevisionSelector = null
)
{
    public AssetRevisionSelector GetRevisionSelectorOrUseCurrent()
    {
        return RevisionSelector ?? AssetRevisionSelector.Current();
    }
};