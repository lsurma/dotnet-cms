using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Surma.Cms.Core.Routing;

public class CmsRouteValueTransformer : DynamicRouteValueTransformer
{
    private readonly CmsManager _cmsManager;
    
    public CmsRouteValueTransformer(CmsManager cmsManager)
    {
        _cmsManager = cmsManager;
    }

    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        var cancellationToken = httpContext.RequestAborted;
        var request = httpContext.Request;
        
        if(request.Path.StartsWithSegments(Consts.PublicStaticAssetRequestPathPrefix))
        {
            values["page"] = Consts.CmsPagePath;
            values[Consts.RequestHandlingMode] = Consts.RequestHandlingModes.StaticAsset;
            return values;
        }
        
        if(request.Path.StartsWithSegments(Consts.InternalApiRequestPathPrefix))
        {
            values["page"] = Consts.CmsPagePath;
            values[Consts.RequestHandlingMode] = Consts.RequestHandlingModes.InternalApi;
            return values;
        }
        
        // Try match page
        var page = await _cmsManager.FindMatchingPageAsync(new PageMatcherFilter(request), cancellationToken);
        
        if (page != null)
        {
            values[Consts.RequestHandlingMode] = Consts.RequestHandlingModes.Page;
            values[Consts.PageMatchLookupKey] = page;
            
            // Check if found page is pointing to local razor page
            // if so, then set the page name to the route value
            if(page.Page.RazorPagePath != null)
            {
                values["page"] = page.Page.RazorPagePath;
                return values;
            }
            else
            {
                // If not, then set the page name to the default page
                values["page"] = Consts.CmsPagePath;
            }
            
            return values;
        }
        
        // If no dynamic route matches, return null to allow the request to fall through to a 404
        return (RouteValueDictionary)null!;
    }
}