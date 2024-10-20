using Microsoft.AspNetCore.Http;
using Surma.Cms.Core.Assets;

namespace Surma.Cms.Core;

public static class HttpRequestsExtensions
{
    // Get reveision selectom from request query param using const key
    public static AssetRevisionSelector? GetRevisionSelector(this HttpRequest request)
    {
        var isDirectPreview = request.Query.TryGetValue(Consts.RevisionSelectorQueryParamName, out var previewKey);
        
        if (isDirectPreview && !String.IsNullOrWhiteSpace(previewKey))
        {
            return new AssetRevisionSelector(previewKey!);
        }
        
        return null;
    }
}