using Surma.Cms.Core.DynamicPages;

namespace Surma.Cms.UI.Blazor.DAL;

public interface ICmsBlazorDataService
{
    public Task<TReturn> HandleAsync<TReturn>(Func<IPageRepository, Task<TReturn>> func, CancellationToken cancellationToken = default);
    
}