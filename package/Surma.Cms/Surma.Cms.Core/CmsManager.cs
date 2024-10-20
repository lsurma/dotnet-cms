using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Surma.Cms.Core.Assets;
using Surma.Cms.Core.DynamicPages;
using Surma.Cms.Core.Routing;

namespace Surma.Cms.Core;

public class CmsManager : ICmsManager
{
    public IServiceScopeFactory ServiceScopeFactory { get; }

    public CmsManager
    (
        IServiceScopeFactory serviceScopeFactory
    )
    {
        ServiceScopeFactory = serviceScopeFactory;
    }


    protected bool IsInitialized;

    protected IList<Page> Pages { get; set; } = new List<Page>();

    public async Task<PageMatchResult?> FindMatchingPageAsync(PageMatcherFilter filter, CancellationToken cancellationToken = default)
    {
        return (await FindMatchingPagesAsync(filter, cancellationToken)).FirstOrDefault();
    }

    public async Task<IEnumerable<PageMatchResult>> FindMatchingPagesAsync(PageMatcherFilter filter, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        var path = filter.Path.Value ?? "/";
        path = path.TrimStart('/');
        path = path.TrimEnd('/');    
        path = "/" + path;

        var matchingRoutes = Pages
            .Select(r => new PageMatchResult(r, MatchRoute(r.RouteTemplate, path)))
            .Where(x => x.RouteMatchData.IsMatch)
            .OrderByDescending(x => x.RouteMatchData.Precedence)
            .ThenBy(x => x.RouteMatchData.ParameterCount - x.RouteMatchData.OptionalParameterCount)
            .ThenBy(x => x.RouteMatchData.OptionalParameterCount);

        return matchingRoutes;
    }

    public async Task ResetDataAsync(CancellationToken cancellationToken = default)
    {
        await RefreshPagesAsync(cancellationToken);
        var assetIdsToLoad = Pages
            .Select(p => p.ContentAssetId)
            .Where(id => id.HasValue)
            .Cast<Guid>()
            .Distinct()
            .ToList();
        
        using var scope = ServiceScopeFactory.CreateScope();
        var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var assets = await assetRepository.GetListAsync(q => assetIdsToLoad.Contains(q.Id), cancellationToken);
        await assetRepository.LoadRevisionsContentAsync(assets.Select(q => q.CurrentRevision).ToList()!, cancellationToken);
        
        var existingViewNames = new List<string>();
        foreach (var page in Pages)
        {
            var viewName = await GetViewNameAsync(page, null, cancellationToken);

            if (viewName == null)
            {
                continue;
            }

            var pageAsset = assets.FirstOrDefault(q => q.Id == page.ContentAssetId);
            var currentRevision = pageAsset?.CurrentRevision;
            var lastModified = currentRevision?.GetLastModificationDate() ?? DateTime.MinValue;
            
            var viewInfo = DynamicRazorViewContentProvider.ViewInfos[viewName];
            var oldViewData = viewInfo.ViewData;
            
            viewInfo.ViewData = new DynamicViewData(
                viewName,
                currentRevision?.Content ?? [],
                currentRevision?.ContentLength ?? 0,
                lastModified,
                true
            );

            // Trigger change if the content has changed
            if (oldViewData?.LastModified != lastModified)
            {
                DynamicRazorViewContentProvider.TryTriggerChange(viewName);
            }

            existingViewNames.Add(viewName);
        }

        // Remove old content if they are not in the new list
        foreach (var (viewName, viewInfo) in DynamicRazorViewContentProvider.ViewInfos)
        {
            // Skip if view is still in use
            if (existingViewNames.Contains(viewName))
            {
                continue;
            }

            DynamicRazorViewContentProvider.ViewInfos.Remove(viewName, out _);
        }
    }

    public async Task EnsureViewDataForRevisionLoadedAsync
    (
        Page page,
        AssetRevisionSelector revisionSelector,
        CancellationToken cancellationToken = default
    )
    {
        if (page.ContentAssetId is null)
        {
            throw new Exception("Page has no content asset");
        }

        using var scope = ServiceScopeFactory.CreateScope();
        var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var asset = await assetRepository.FindAsync(page.ContentAssetId.Value, revisionSelector, cancellationToken);

        if (asset is null)
        {
            throw new Exception("Content asset not found");
        }

        var viewName = await GetViewNameAsync(page, revisionSelector, cancellationToken);
        var revision = asset.FindRevision(revisionSelector);

        if (revision is null)
        {
            throw new Exception("Content asset revision not found");
        }

        
        DynamicRazorViewContentProvider.ViewInfos[viewName].ViewData = new DynamicViewData(
            viewName,
            revision.Content,
            revision.ContentLength,
            revision.GetLastModificationDate(),
            asset.CurrentRevision == revision
        );
        DynamicRazorViewContentProvider.TryTriggerChange(viewName);
    }

    public async Task<string> GetViewNameAsync
    (
        Page page,
        AssetRevisionSelector? revisionSelector = null,
        CancellationToken cancellationToken = default
    )
    {
        var viewName = $"{Consts.CmsRazorViewNamePrefix}/{page.Slug}";
        
        if (revisionSelector is not null)
        {
            viewName += $":{revisionSelector}";
        }

        return viewName;
    }

    protected async Task RefreshPagesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var pageRepository = scope.ServiceProvider.GetRequiredService<IPageRepository>();
        Pages = (await pageRepository.GetAllAsync(cancellationToken)).ToList();
    }

    protected async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
        {
            return;
        }

        await ResetDataAsync(cancellationToken);
        IsInitialized = true;
    }

    protected RoutePatternMatchResult MatchRoute(string template, string path)
    {
        var routeTemplate = TemplateParser.Parse(template);
        var matcher = new TemplateMatcher(routeTemplate, new RouteValueDictionary());
        var values = new RouteValueDictionary();

        bool isMatch = matcher.TryMatch(path, values);

        int precedence = 0;
        int parameterCount = 0;
        int optionalParameterCount = 0;

        foreach (var segment in routeTemplate.Segments)
        {
            if (segment.IsSimple)
            {
                var segmentPart = segment.Parts[0];

                if (segmentPart.IsLiteral)
                {
                    precedence++;
                }
                else if (segmentPart.IsParameter)
                {
                    parameterCount++;
                    if (segmentPart.IsOptional)
                    {
                        optionalParameterCount++;
                    }
                }
            }
            else
            {
                // For complex segments, count literals and parameters
                precedence += segment.Parts.Count(p => p.IsLiteral);
                var parameters = segment.Parts.Where(q => q.IsParameter).ToList();
                parameterCount += parameters.Count;
                optionalParameterCount += parameters.Count(p => p.IsOptional);
            }
        }

        return new RoutePatternMatchResult(isMatch, precedence, parameterCount, optionalParameterCount, values);
    }
}