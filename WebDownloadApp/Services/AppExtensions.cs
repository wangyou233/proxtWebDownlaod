using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using WebDownload.App.Services.ImpI;

namespace WebDownload.App.Services;

public static class AppExtensions
{
    public static IServiceCollection AddDownloadAppServices(this IServiceCollection services)
    {
        services.AddTransient<IDownloadFileImpl, DownloadFileImpl>();
        return services;
    }

    public static IServiceCollection AddLocalizations(this IServiceCollection services)
    {
        services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

        services.AddPortableObjectLocalization(options => options.ResourcesPath = "Localization");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("zh"),
                new CultureInfo("en")
            };
            options.DefaultRequestCulture = new RequestCulture("zh");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
        return services;

    }
}