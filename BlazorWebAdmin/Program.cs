using AspectCore.Extensions.DependencyInjection;
using BlazorWebAdmin;
using BlazorWebAdmin.Aop;
using BlazorWebAdmin.Auth;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Common;
using Project.Common.Attributes;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.
services.AddRazorPages();
services.AddServerSideBlazor();
services.AddECharts();

services.AutoInjects();

services.AddScoped<StateContainer>();
services.AddScoped<RouterStore>();
services.AddScoped<CounterStore>();
services.AddScoped<UserStore>();
services.AddScoped<EventDispatcher>();
services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
//
services.AddAntDesign();
//�滻Ĭ�ϵ�����
services.AddScoped<LogAop>();
services.ConfigureDynamicProxy(config =>
{
    config.Interceptors.Add(new CustomFactory());
    config.NonAspectPredicates.Add(m => m.CustomAttributes.All(a => a.AttributeType != typeof(LogInfoAttribute)));
});
builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
