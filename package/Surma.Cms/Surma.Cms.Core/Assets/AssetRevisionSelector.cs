namespace Surma.Cms.Core.Assets;

public record AssetRevisionSelector
{
    public AssetRevisionSelector(Guid revisionId)
    {
        RevisionId = revisionId;
    }
    
    public AssetRevisionSelector(string previewKey)
    {
        PreviewKey = previewKey;
    }
    
    private AssetRevisionSelector()
    {
        RevisionId = null;
        PreviewKey = null;
        UseCurrentRevision = true;
    }
    
    public Guid? RevisionId { get; }
    
    public string? PreviewKey { get; }
    
    public bool? UseCurrentRevision { get; set; }
    
    public bool IsMatch(AssetRevision assetRevision)
    {
        if(UseCurrentRevision == true)
        {
            return assetRevision.IsCurrent;
        }
        
        if (RevisionId.HasValue)
        {
            return assetRevision.Id == RevisionId.Value;
        }
        
        if (PreviewKey is not null)
        {
            return assetRevision.PreviewKey == PreviewKey;
        }
        
        return false;
    }

    public override string ToString()
    {
        if(UseCurrentRevision == true)
        {
            return Consts.CurrentAssetRevisionName;
        }
        
        return RevisionId.HasValue ? RevisionId.Value.ToString() : PreviewKey ?? String.Empty;
    }

    public static AssetRevisionSelector Current()
    {
        return new AssetRevisionSelector()
        {
            UseCurrentRevision = true
        };
    }
};