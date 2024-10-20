using Microsoft.Extensions.FileProviders;

namespace Surma.Cms.Core.DynamicPages;

public class DynamicContentFileInfo : IFileInfo
{
    public DynamicContentFileInfo(DynamicViewData viewData)
    {
        Name = viewData.Name;
        UpdateViewContent(viewData);
    }
    
    public void UpdateViewContent(DynamicViewData viewData)
    {
        ViewContent = viewData.Content ?? Array.Empty<byte>();
        LastModified = viewData.LastModified;
        Length = viewData.Length;
    }

    public string Name { get; }

    public string? PhysicalPath => null;

    public byte[] ViewContent { get; private set; } = null!;

    public bool Exists => true;

    public bool IsDirectory => false;

    public DateTimeOffset LastModified { get; private set; }

    public long Length { get; private set; }

    public Stream CreateReadStream()
    {
        return new MemoryStream(ViewContent);
    }
}