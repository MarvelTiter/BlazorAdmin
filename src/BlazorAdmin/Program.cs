using BlazorAdmin;
using BlazorAdmin.TestPages;
using Project.AppCore;
using Project.Constraints;
using Project.Services;
using Project.UI.AntBlazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(option =>
    {
        option.MaximumReceiveMessageSize = 1024 * 1024 * 2;
    });

builder.Services.AddAntDesignUI();
builder.AddProject(setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    setting.ConfigurePage(locator =>
    {
        locator.SetPage<TestPage4>("LocatorTest");
    });
    setting.ConfigureSettingProviderType<CustomSetting>();
    setting.AddInterceotor<AdditionalTest>();
});

builder.AddDefaultLightOrm();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseProject();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. AppConst.Pages]);

app.Run();
