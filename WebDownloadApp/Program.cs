using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using WebDownload.App.Configs;
using WebDownload.App.Hubs;
using WebDownload.App.Jobs;
using WebDownload.App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
var proxyConfig = builder.Configuration.GetSection("ProxySetting").Get<ProxyConfig>();

builder.Services.AddSingleton<ProxyConfig>(proxyConfig);

builder.Services.AddLocalizations();

builder.Services.AddHostedService<DeleteExpiredFileJob>();

builder.Services.AddDownloadAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});
app.UseRequestLocalization();
app.MapHub<DownloadNotifyHub>("/download/hub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
