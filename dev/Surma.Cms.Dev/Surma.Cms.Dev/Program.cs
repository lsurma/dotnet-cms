using Microsoft.EntityFrameworkCore;
using Surma.Cms.Core;
using Surma.Cms.Core.Routing;
using Surma.CMS.Dev.DAL;
using Surma.CMS.Dev.Scrutor;
using Surma.CMS.Dev.Smietnik;
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContextPool<AppDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.EnableSensitiveDataLogging(true);
});
builder.Services.AddDbContextFactory<AppDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.EnableSensitiveDataLogging(true);
});

builder.Services.Scan(scan => {
    scan.FromAssembliesOf(typeof(ITransient))
        .AddClasses(classes => classes.AssignableTo<ITransient>())
            .AsImplementedInterfaces()
            .AsSelf()
            .WithTransientLifetime();
    
    scan.FromAssembliesOf(typeof(IEventHandler<>))
        .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime();
});

builder.Services
    .AddRazorPages(config => {
        config.RootDirectory = "/Pages/Site1";
    })
    .AddRazorCmsPages();



// // Add services to the container.
// builder.Services.AddRazorPages(options => {
// }).AddRazorRuntimeCompilation(options => {
//     var provider = options.FileProviders.First();
//     // options.FileProviders.Add(new CompositeFileProvider(provider, new DynamicContentFileProvider()));
//     // options.FileProviders.Add(new DynamicContentFileProvider());
// });
//     

//
// services.AddSingleton<DynamicRouteManager>();
// services.AddSingleton<IDynamicRouteService, DynamicRouteService>();
// services.AddSingleton<DynamicRouteTransformer>();

// builder.Services.AddSingleton<Transformer2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();
app.MapRazorCmsPages();

app.Run();