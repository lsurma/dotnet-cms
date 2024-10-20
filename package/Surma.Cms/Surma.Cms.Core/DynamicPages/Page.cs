namespace Surma.Cms.Core.DynamicPages;

public class Page
{
    public Guid Id { get; set; }
    
    // TODO: Rename to "DisplayName"
    public required string Name { get; set; }

    // TODO: Rename to "Name"
    public required string Slug { get; set; }
    
    public string Host { get; set; } = "*";

    public required string RouteTemplate { get; set; }
    
    public string? RazorPagePath { get; set; }

    public Guid? ContentAssetId { get; set; }
    
    public bool IsPublished { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}
