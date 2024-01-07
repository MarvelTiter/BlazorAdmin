using Demo.Web.Components;
using MDbContext;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.Sqlite;
using Project.AppCore;
using Project.AppCore.Auth;
using Project.AppCore.Routers;
using Project.AppCore.Store;
using Project.Constraints;
using Project.Constraints.Store;
using Project.Constraints.UI;
using Project.Services;
using Project.UI.AntBlazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddProject(setting =>
{
    setting.SettingProviderType = typeof(CustomSetting);
});

builder.AddDefaultLightOrm();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseProject();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. Config.Pages]);

app.Run();