using System.Collections.Concurrent;

namespace Surma.Cms.Core.DynamicPages;

public class DynamicRazorViewInfo
{
    public DynamicRazorViewInfo()
    {
    }
    
    public bool HasViewContent => ViewData?.Content != null;
    
    public DynamicViewData? ViewData { get; set; }

    public DynamicContentFileInfo? FileInfo { get; set; }
    
    public DateTimeOffset LastAccessTime { get; set; }
}

public class DynamicRazorViewInfoDictionary : ConcurrentDictionary<string, DynamicRazorViewInfo>
{
    public DynamicRazorViewInfoDictionary() : base()
    {
    }
    
    // indexer
    public new DynamicRazorViewInfo this[string key]
    {
        get
        {
            if (!TryGetValue(key, out var value))
            {
                value = new DynamicRazorViewInfo();
                this[key] = value;
            }
            return value;
        }
        
        set
        {
            base[key] = value;
        }
    }
}