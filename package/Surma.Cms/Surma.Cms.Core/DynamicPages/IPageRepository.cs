namespace Surma.Cms.Core.DynamicPages;

public interface IPageRepository
{
    Task<IEnumerable<Page>> GetAllAsync(CancellationToken cancellationToken);
    
    Task<Page?> FindAsync(Guid id, CancellationToken cancellationToken);
    
    Task<Page> UpdateAsync(Page page, bool autoSave, CancellationToken cancellationToken);
    
    Task<Page> InsertAsync(Page page, bool autoSave, CancellationToken cancellationToken);
    
}