using OliveMorocco.Business;
using OliveMorocco.DataAccess;
using OliveMorocco.Web.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options => options.ViewLocationExpanders.Add(new SectionViewLocationExpander()));
builder.Services.AddBusiness(
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found."));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<IAppDatabaseInitializer>().InitializeAsync();

app.Run();
