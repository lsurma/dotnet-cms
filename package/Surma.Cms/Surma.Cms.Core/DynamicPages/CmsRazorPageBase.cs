using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Surma.Cms.Core.Assets;
using Surma.Cms.Core.Routing;
using Surma.Cms.Core.StaticAssets;

namespace Surma.Cms.Core.DynamicPages;

public class CmsRazorPageBase : PageModel
{
    protected Dictionary<Type, object> LazyServices { get; set; } = new Dictionary<Type, object>();
    
    protected CmsManager CmsManager => GetLazyService<CmsManager>();

    protected IStaticAssetsManager StaticAssetsManager => GetLazyService<IStaticAssetsManager>();
    
    protected ILocalEventBus LocalEventBus => GetLazyService<ILocalEventBus>();

    protected Dictionary<string, AssetRevisionSelector> RevisionSelectors { get; set; } = new Dictionary<string, AssetRevisionSelector>();

    protected readonly string GlobalRevisionSelectorKey = "_global";
    
    protected AssetRevisionSelector? GlobalRevisionSelector => RevisionSelectors.GetValueOrDefault(GlobalRevisionSelectorKey);
    
    protected CancellationToken CancellationToken => HttpContext.RequestAborted;

    public virtual Task<IActionResult> OnGetAsync()
    {
        return HandleRequestAsync();
    }

    protected virtual async Task<IActionResult> HandleRequestAsync()
    {
        var requestHandlingMode = RouteData.Values[Consts.RequestHandlingMode] as string;

        if (!String.IsNullOrWhiteSpace(requestHandlingMode))
        {
            await PrepareAsync(requestHandlingMode); 
        }

        if (requestHandlingMode == Consts.RequestHandlingModes.StaticAsset)
        {
            return await HandleStaticAssetRequestAsync();
        }
        
        if(requestHandlingMode == Consts.RequestHandlingModes.InternalApi)
        {
            return await HandleInternalApiRequestAsync();
        }

        if (requestHandlingMode == Consts.RequestHandlingModes.Page)
        {
            return await HandlePageRequestAsync();
        }
        
        return NotFound();
    }
    
    protected virtual async Task<IActionResult> HandlePageRequestAsync()
    {
        // Check if page is already matched in dynamic route transformer, if not, try to match it
        RouteData.Values.TryGetValue(Consts.PageMatchLookupKey, out var pageMatchObj);
        var pageMatch = (PageMatchResult?)pageMatchObj ?? await CmsManager.FindMatchingPageAsync(
            new PageMatcherFilter(HttpContext.Request), 
            CancellationToken
        );

        if (pageMatch != null)
        {
            var page = pageMatch.Page;
            pageMatch.FillRouteData(RouteData);

            var viewName = await CmsManager.GetViewNameAsync(page, GlobalRevisionSelector, CancellationToken);

            // if preview mode, ensure that page is loaded
            if (GlobalRevisionSelector != null)
            {
                await CmsManager.EnsureViewDataForRevisionLoadedAsync(page, GlobalRevisionSelector, CancellationToken);
            }
            
            // Add check if page is found and can be displayed
            if (viewName != null)
            {
                return Partial(viewName, this);
            }

            return NotFound();
        }
        else
        {
            return NotFound();
        }
    }

    protected virtual Task PrepareAsync(string? requestHandlingMode = null)
    {
        // Handle preview mode - global for all used assets
        var globalRevisionSelector = HttpContext.Request.GetRevisionSelector();
        if (globalRevisionSelector != null)
        {
            RevisionSelectors[GlobalRevisionSelectorKey] = globalRevisionSelector;
        }
        
        return Task.CompletedTask;
    }
    
    protected virtual async Task<IActionResult> HandleInternalApiRequestAsync()
    {
        await CmsManager.ResetDataAsync(CancellationToken);
        return new EmptyResult();
    }
    
    protected virtual async Task<IActionResult> HandleStaticAssetRequestAsync()
    {
        // Invoke event start
        var path = Request.Path.Value ?? "/";
        var assetName = Path.GetFileNameWithoutExtension(path);
        var assetNameWithoutPrefix = assetName.Replace(Consts.PublicStaticAssetRequestPathPrefix, "");
        var revisionSelector = HttpContext.Request.GetRevisionSelector();
        
        var staticAssetData = await StaticAssetsManager.FindAssetDataAsync(
            new StaticAssetFilter(assetNameWithoutPrefix, revisionSelector), 
            CancellationToken
        );
        
        if(staticAssetData == null)
        {
            return NotFound();
        }
        
        HttpContext.Response.Headers.CacheControl = staticAssetData.IsCurrentRevision ? "public, max-age=3600" : "no-cache";

        var result = new ContentResult()
        {
            ContentType = staticAssetData.ContentType,
            Content = Encoding.UTF8.GetString(staticAssetData.Content ?? []),
            StatusCode = 200
        };
        
        // invoke event with prepared result before returning it
        return result;
    }

    protected T GetLazyService<T>() where T : class
    {
        if (!LazyServices.TryGetValue(typeof(T), out var service))
        {
            service = HttpContext.RequestServices.GetRequiredService<T>();
            LazyServices[typeof(T)] = service;
        }

        return (T)service;
    }
}