using Microsoft.Extensions.DependencyInjection;
using Surma.Cms.Core.DynamicPages;

namespace Surma.Cms.UI.Blazor.DAL;

public class CmsBlazorDataService : ICmsBlazorDataService
{
    protected IServiceProvider ServiceProvider { get; }
    
    public CmsBlazorDataService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
    
    public async Task<TReturn> HandleAsync<TReturn>(Func<IPageRepository, Task<TReturn>> func, CancellationToken cancellationToken = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var pageRepository = scope.ServiceProvider.GetRequiredService<IPageRepository>();
        return await func(pageRepository);
    }
}