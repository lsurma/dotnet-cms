namespace Surma.Cms.Core;

public class Consts
{
    public static string PublicStaticAssetRequestPathPrefix = "/cms-assets";

    public static string InternalApiRequestPathPrefix = "/_cms-internal-api";

    public static string RequestHandlingMode = "_cmsRequestHandlingMode";
    
    public static string PageMatchLookupKey = "_cmsPageMatch";
    
    public static string RevisionSelectorQueryParamName = "revision";
    
    public static string CmsRazorViewNamePrefix = "/cms";

    public static string CmsPagePath = "/CmsPage";
    
    public static string CurrentAssetRevisionName = "current";

    public class RequestHandlingModes
    {
        public static string StaticAsset = nameof(StaticAsset);
        public static string Page = nameof(Page);
        public static string InternalApi = nameof(InternalApi);
    }
}