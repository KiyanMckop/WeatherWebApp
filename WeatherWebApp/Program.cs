using OfficeOpenXml;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configure EPPlus license for non-commercial use
ExcelPackage.License.SetNonCommercialOrganization("Personal Project");

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<WeatherWebApp.Services.WeatherService>();
builder.Services.AddScoped<WeatherWebApp.Services.ExportService>();

var app = builder.Build();

// Configure forwarded headers from reverse proxy (nginx)
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// Allow forwarded headers from the local proxy (127.0.0.1)
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
forwardedOptions.KnownProxies.Add(IPAddress.Loopback);

app.UseForwardedHeaders(forwardedOptions);

// If the proxy strips the prefix, some proxies set the X-Forwarded-Prefix
// header. Honor that header so the app can generate correct URLs.
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var prefixValues))
    {
        var prefix = prefixValues.ToString().TrimEnd('/');
        if (!string.IsNullOrEmpty(prefix))
        {
            context.Request.PathBase = prefix;
        }
    }
    await next();
});

// If the app is hosted under a sub-path, set the PathBase early so
// StaticFiles and routing generate urls correctly.
app.UsePathBase("/weatherapp");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Serve static files (will respect PathBase or X-Forwarded-Prefix)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Weather}/{action=Index}/{id?}");

app.Run();
