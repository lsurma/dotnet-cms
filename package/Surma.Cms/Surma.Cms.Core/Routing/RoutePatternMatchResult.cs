using Microsoft.AspNetCore.Routing;

namespace Surma.Cms.Core.Routing;

public record RoutePatternMatchResult(bool IsMatch, int Precedence, int ParameterCount, int OptionalParameterCount, RouteValueDictionary RouteValueDictionary);
