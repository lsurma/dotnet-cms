using Microsoft.AspNetCore.Http;

namespace Surma.Cms.Core.Routing;

public record PageMatcherFilter
(
    HostString Host,
    PathString Path,
    QueryString? QueryString,
    IHeaderDictionary? Headers
)
{
    public PageMatcherFilter(HttpRequest request)
        : this(request.Host, request.Path, request.QueryString, request.Headers)
    {
        
    }
};