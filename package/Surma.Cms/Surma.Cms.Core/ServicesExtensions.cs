using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Surma.Cms.Core.DynamicPages;
using Surma.Cms.Core.Routing;
using Surma.Cms.Core.StaticAssets;

namespace Surma.Cms.Core;

public static class ServicesExtensions
{
    public static IMvcBuilder AddRazorCmsPages(this IMvcBuilder builder, Action<MvcRazorRuntimeCompilationOptions>? setupAction = null)
    {
        builder.AddRazorRuntimeCompilation(options => {
            options.FileProviders.Add(new DynamicRazorViewContentProvider());

            setupAction?.Invoke(options);
        });
        builder.Services.AddSingleton<CmsManager>();
        builder.Services.AddSingleton<IStaticAssetsManager, StaticAssetsManager>();
        builder.Services.AddSingleton<CmsRouteValueTransformer>();
        builder.Services.AddHttpContextAccessor();
        
        return builder;
    }

    public static IEndpointRouteBuilder MapRazorCmsPages(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDynamicPageRoute<CmsRouteValueTransformer>("{**CmsRouteValueTransformer}");
        return endpoints;
    }

    // public static IServiceCollection AddCMS(this IServiceCollection services)
    // {
    //     services.AddSingleton<CMSManager>();
    //
    //     services.Scan(scan =>
    //     {
    //         scan.FromAssembliesOf(typeof(ITransient))
    //             .AddClasses(classes => classes.AssignableTo<ITransient>())
    //                 .AsImplementedInterfaces()
    //                 .AsSelf()
    //                 .WithTransientLifetime();
    //
    //         scan.FromAssembliesOf(typeof(IEventHandler<>))
    //             .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
    //                 .AsImplementedInterfaces()
    //                 .WithTransientLifetime();
    //     });
    //
    //     return services;
    // }
}