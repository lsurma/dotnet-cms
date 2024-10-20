using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Surma.Cms.Core.DynamicPages;
using Surma.Cms.Dev.Blazor.Components;
using Surma.Cms.Dev.Blazor.Components.Dynamic;
using Surma.Cms.Dev.Blazor.Hubs;
using Surma.Cms.Dev.DAL;
using Surma.CMS.Dev.DAL;
using Surma.CMS.Dev.Smietnik;
using Surma.Cms.UI.Blazor.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
   opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
       ["application/octet-stream"]);
});

builder.Services.AddScoped<IComponentInterface, TestComponent>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

// Add DbContext
builder.Services.AddDbContextPool<AppDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.EnableSensitiveDataLogging(true);
});
builder.Services.AddDbContextFactory<AppDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.EnableSensitiveDataLogging(true);
});

builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<ICmsBlazorDataService, CmsBlazorDataService>();

var app = builder.Build();
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<AppHub>("/apphub");
app.Run();