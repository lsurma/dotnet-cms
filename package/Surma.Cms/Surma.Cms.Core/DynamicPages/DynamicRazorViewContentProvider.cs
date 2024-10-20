using System.Collections.Concurrent;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Surma.Cms.Core.DynamicPages;

public class DynamicRazorViewContentProvider : IDynamicRazorViewContentProvider
{
    protected readonly static string PathPrefix = Consts.CmsRazorViewNamePrefix;

    protected List<string> ExcludedPaths = [$"{PathPrefix}/_viewimports.cshtml"];
    
    public static DynamicRazorViewInfoDictionary ViewInfos { get; } = new DynamicRazorViewInfoDictionary();
    
    public static IDictionary<string, DynamicContentChangeToken> ChangeTokens { get; } = new ConcurrentDictionary<string, DynamicContentChangeToken>();
    
    public DynamicRazorViewContentProvider()
    {
    }
    
    public static bool TryTriggerChange(string viewName)
    {
        if (!ViewInfos.TryGetValue(viewName, out var viewInfo) || !ChangeTokens.TryGetValue(viewName, out var changeToken))
        {
            return false;
        }
        changeToken.TriggerChange();
        return true;
    }
    
    public IFileInfo GetFileInfo(string subpath)
    {
        var subpathNormalized = subpath.ToLower() ?? string.Empty;

        if (!ViewInfos.TryGetValue(subpathNormalized, out var viewInfo))
        {
            return new NotFoundFileInfo(subpath);
        }
        
        if (!viewInfo.HasViewContent)
        {
            return new NotFoundFileInfo(subpath);
        }

        // Save last access time
        viewInfo.LastAccessTime = DateTimeOffset.Now;
            
        if (viewInfo.FileInfo != null)
        {
            viewInfo.FileInfo.UpdateViewContent(viewInfo.ViewData!);
            return viewInfo.FileInfo;
        }
        
        ViewInfos[subpathNormalized].FileInfo = new DynamicContentFileInfo(viewInfo.ViewData!);
        
        return ViewInfos[subpathNormalized].FileInfo!;
    }

    public IChangeToken Watch(string filter)
    {
        var filterNormalized = filter.ToLower() ?? string.Empty;
        
        if (!ExcludedPaths.Contains(filterNormalized) && filterNormalized.StartsWith(PathPrefix))
        {
            DynamicContentChangeToken? changeToken = null;
            
            if(!ChangeTokens.TryGetValue(filterNormalized, out changeToken))
            {
                changeToken = DynamicContentChangeToken.WithActiveChangeCallback(filter);
            }
            
            ChangeTokens[filterNormalized] = changeToken;
            
            return changeToken;
        }

        return DynamicContentChangeToken.NeverChanging(filter);
    }
    
    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return new NotFoundDirectoryContents();
    }
    
}