using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Surma.Cms.Core.Assets;

public class AssetRevision
{
    public Guid Id { get; set; }
    
    public Guid AssetId { get; set; }
    
    public Asset Asset { get; set; }

    public int Number { get; set; }
    
    public string? Description { get; set; }
    
    public string? PreviewKey { get; set; }

    
    [NotMapped]
    public bool IsContentLoaded { get; set; }
    
    private byte[]? _content;

    public byte[]? Content
    {
        get
        {
            if (IsContentLoaded)
            {
                return _content;
            }

            throw new InvalidOperationException("Content was not loaded");
        }
        
        set => _content = value;
    }

    public required string ContentType { get; set; }
    
    public long ContentLength { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }

    
    [NotMapped]
    public bool IsCurrent => Asset.CurrentRevisionId == Id;
    
    public string GetName()
    {
        if(IsCurrent)
        {
            return Consts.CurrentAssetRevisionName;
        }
        
        return !String.IsNullOrWhiteSpace(PreviewKey) ? PreviewKey : Id.ToString();
    }
    
    public DateTimeOffset GetLastModificationDate()
    {
        return UpdatedAt ?? CreatedAt;
    }
    
    public string? GetContentAsString()
    {
        if (Content is null)
        {
            return null;
        }
        
        return Encoding.UTF8.GetString(Content);
    }
    
    public void SetContent(string? content)
    {
        if (content is null)
        {
            Content = null;
            ContentLength = 0;
            return;
        }
        
        Content = Encoding.UTF8.GetBytes(content);
        ContentLength = Content.Length;
    }
}
