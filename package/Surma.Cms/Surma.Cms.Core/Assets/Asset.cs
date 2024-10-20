using System.Collections.ObjectModel;
using System.Text;

namespace Surma.Cms.Core.Assets;

public class Asset
{
    public Guid Id { get; set; }
    
    public required string DisplayName { get; set; }

    public required string Name { get; set; }
    
    public required string Type { get; set; }
    
    public bool IsStatic { get; set; }
    
    public Guid? CurrentRevisionId { get; set; }

    public AssetRevision? CurrentRevision { get; set; }
    
    public ICollection<AssetRevision> Revisions { get; set; } = new Collection<AssetRevision>();
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public string? GetCurrentContentAsString()
    {
        if (CurrentRevision is null)
        {
            return null;
        }
        
        return CurrentRevision.GetContentAsString();
    }
    
    public void SetCurrentContent(string? content)
    {
        if (CurrentRevision is null)
        {
            var rev = new AssetRevision
            {
                Id = Guid.NewGuid(),
                Number = 1,
                ContentType = "text/plain",
                CreatedAt = DateTimeOffset.UtcNow
            };
            rev.SetContent(content);
            Revisions.Add(rev);
        }
        
    }
    
    public AssetRevision? FindRevision(AssetRevisionSelector revisionSelector)
    {
        return Revisions.FirstOrDefault(revisionSelector.IsMatch);
    }
}

