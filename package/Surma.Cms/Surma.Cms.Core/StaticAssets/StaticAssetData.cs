namespace Surma.Cms.Core.StaticAssets;

public class StaticAssetData
(
    string name,
    byte[]? content,
    long length,
    string contentType,
    DateTimeOffset lastModificationDate,
    string revisionName,
    bool isCurrentRevision    
)
{
    public string Name { get; } = name;

    public string RevisionName { get; } = revisionName;
    
    public byte[]? Content { get; } = content;
    
    public long Length { get; } = length;
    
    public DateTimeOffset LastModificationDate { get; } = lastModificationDate;
    
    public string ContentType { get; } = contentType;

    public bool IsCurrentRevision { get; } = isCurrentRevision;
    
    public string ETag => $"{LastModificationDate.Ticks:x}";
}