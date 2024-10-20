namespace Surma.Cms.Core.DynamicPages;

public class DynamicViewData
(
    string name,
    byte[]? content,
    long length,
    DateTimeOffset lastModified,
    bool isCurrentRevision    
)
{
    public string Name { get; } = name;
    
    public byte[]? Content { get; } = content;
    
    public long Length { get; } = length;
    
    public DateTimeOffset LastModified { get; } = lastModified;
    
    public bool IsCurrentRevision { get; } = isCurrentRevision;
}