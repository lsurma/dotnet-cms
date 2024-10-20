using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Surma.CMS.Dev.Smietnik;

public class Transformer1 : DynamicRouteValueTransformer
{
    public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        var path = httpContext.Request.Path.Value;

        if (path == "/test-route-1")
        {
            values["page"] = "/Privacy";
            values["routeTemplate"] = "template";

            return new ValueTask<RouteValueDictionary>(values);
        }

        // If no dynamic route matches, return null to allow the request to fall through to a 404
        return new ValueTask<RouteValueDictionary>((RouteValueDictionary)null);
    }
}

public class Transformer2 : DynamicRouteValueTransformer
{
    public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
    {
        var path = httpContext.Request.Path.Value;

        if (path == "/test-route-2")
        {
            values["page"] = "/Privacy";
            return new ValueTask<RouteValueDictionary>(values);
        }

        // If no dynamic route matches, return null to allow the request to fall through to a 404
        return new ValueTask<RouteValueDictionary>((RouteValueDictionary)null);
    }
}