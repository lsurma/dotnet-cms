using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Surma.Cms.Core.Assets;

namespace Surma.Cms.Core.StaticAssets;

[HtmlTargetElement("script", Attributes = "name")]
[HtmlTargetElement("css", Attributes = "name")]
public class CmsStaticAssetTagHelper : TagHelper
{
    protected IStaticAssetsManager StaticAssetsManager { get; }
    
    protected IHttpContextAccessor HttpContextAccessor { get; }

    // This attribute will control whether the GUID is appended or not
    public string Name { get; set; } = "";

    public CmsStaticAssetTagHelper(
        IStaticAssetsManager staticAssetsManager,
        IHttpContextAccessor httpContextAccessor
    )
    {
        StaticAssetsManager = staticAssetsManager;
        HttpContextAccessor = httpContextAccessor;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (String.IsNullOrWhiteSpace(Name))
        {
            output.SuppressOutput();
            return;
        }
        
        // Check if revision/preview mode is enabled
        var revisionSelector = HttpContextAccessor.HttpContext?.Request.GetRevisionSelector() ?? AssetRevisionSelector.Current();

        // Find the asset data
        var assetData = await StaticAssetsManager.FindAssetDataAsync(new StaticAssetFilter(Name, revisionSelector));
        
        if(assetData == null)
        {
            output.SuppressOutput();
            return;
        }
        
        if (output.TagName == "css")
        {
            var url = $"{Consts.PublicStaticAssetRequestPathPrefix}/{Name}.css?{Consts.RevisionSelectorQueryParamName}={assetData.RevisionName}&ETag={assetData.ETag}";
    
            output.TagName = "link";
            output.Attributes.SetAttribute("rel", "stylesheet");
            output.Attributes.SetAttribute("href", url);

            // if (!string.IsNullOrEmpty(url))
            // {
            //     var separator = url.Contains("?") ? "&" : "?";
            //     var guid = Guid.NewGuid().ToString();
            //
            //     // Append the GUID as a query string parameter
            //     url += $"{separator}id={Id}:{guid}";
            //
            //     // Set the modified URL back to the attribute
            //     output.Attributes.SetAttribute(attributeName, url);
            // }
        }
        
        
    }
}