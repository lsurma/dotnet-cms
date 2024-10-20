using Microsoft.EntityFrameworkCore;
using Surma.Cms.Core.DynamicPages;
using Surma.CMS.Dev.DAL;
using Surma.CMS.Dev.Scrutor;

namespace Surma.Cms.Dev.DAL;

public class PageRepository : IPageRepository, ITransient
{
    public AppDbContext DbContext { get; }

    public PageRepository
    (
        AppDbContext dbContext
    )
    {
        DbContext = dbContext;
    }

    public async Task<IEnumerable<Page>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await DbContext.Pages.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Page?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var page = await DbContext.Pages.FirstOrDefaultAsync(cancellationToken);

        if(page is not null)
        {
            await DbContext.Entry(page).ReloadAsync(cancellationToken);
        }
        
        return page;
    }
    
    public async Task<Page> UpdateAsync(
        Page page, 
        bool autoSave = true,
        CancellationToken cancellationToken = default
    )
    {
        page.UpdatedAt = DateTime.UtcNow;

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        
        return page;
    }
    
    public async Task<Page> InsertAsync(Page page, bool autoSave, CancellationToken cancellationToken)
    {
        page.CreatedAt = DateTime.UtcNow;
        page.UpdatedAt = DateTime.UtcNow;
        
        DbContext.Pages.Add(page);
        
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        
        return page;
    }
}