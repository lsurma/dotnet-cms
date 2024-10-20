using Microsoft.AspNetCore.Routing;
using Surma.Cms.Core.DynamicPages;

namespace Surma.Cms.Core.Routing;

public record PageMatchResult
(
    Page Page,
    RoutePatternMatchResult RouteMatchData
)
{
    public void FillRouteData(RouteData routeData)
    {
        foreach (var (key, value) in RouteMatchData.RouteValueDictionary)
        {
            routeData.Values[key] = value;
        }
    }
};