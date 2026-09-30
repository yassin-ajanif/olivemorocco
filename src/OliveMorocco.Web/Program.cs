using Microsoft.Extensions.FileProviders;
using OliveMorocco.Business;
using OliveMorocco.DataAccess;
using OliveMorocco.Web.Logging;
using OliveMorocco.Web.Photos;
using OliveMorocco.Web.Routing;

var builder = WebApplication.CreateBuilder(args);

var logFilePath = builder.Configuration["Logging:File:Path"];
if (!string.IsNullOrWhiteSpace(logFilePath))
{
    var resolvedLogPath = Path.IsPathRooted(logFilePath)
        ? logFilePath
        : Path.Combine(builder.Environment.ContentRootPath, logFilePath);
    builder.Logging.AddProvider(new FileLoggerProvider(resolvedLogPath));
}

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options => options.ViewLocationExpanders.Add(new SectionViewLocationExpander()));
builder.Services.AddBusiness(
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found."));
builder.Services.AddSingleton<IPhotoStore, PhotoStore>();

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

// Product photos, written by PhotoStore outside wwwroot. Mounted as a second static
// file provider so uploads live in a directory a rebuild cannot wipe.
//
// nosniff matters here: the path segment is user-supplied in the sense that anyone who
// can reach a product form can choose what gets written, and without it a browser could
// be talked into treating an image as script. Filenames are month-sharded GUIDs, so a
// given URL always means the same bytes and can be cached hard.
var photoStore = app.Services.GetRequiredService<IPhotoStore>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(photoStore.Root),
    RequestPath = photoStore.UrlPrefix,
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.XContentTypeOptions = "nosniff";
        context.Context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
    },
});

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

var startupLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
try
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<IAppDatabaseInitializer>().InitializeAsync();
}
catch (Exception ex)
{
    startupLogger.LogError(ex, "Database initialization failed");
    throw;
}

app.Run();
