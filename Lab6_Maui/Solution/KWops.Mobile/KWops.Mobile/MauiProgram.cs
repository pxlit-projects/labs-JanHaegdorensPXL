using KWops.Mobile;
using CommunityToolkit.Maui;
using KWops.Mobile.Views;
using KWops.Mobile.ViewModels;
using KWops.Mobile.Services.Identity;
using KWops.Mobile.Services;
using KWops.Mobile.Settings;
using KWops.Mobile.Services.Backend;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        RegisterDependencies(builder.Services);

        return builder.Build();
    }

    private static void RegisterDependencies(IServiceCollection services)
    {
        // Register your dependencies here

        // Views
        services.AddTransient<LoginPage>();
        services.AddTransient<TeamsPage>();
        services.AddTransient<TeamDetailPage>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<TeamsViewModel>();
        services.AddTransient<TeamDetailViewModel>();

        // Services
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<INavigationService, NavigationService>();
        services.AddTransient<IToastService, ToastService>();
        services.AddTransient<ITeamsService, TeamsService>();
        services.AddTransient<IBackendService, BackendService>();

        

        // Other
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<IAppSettings, DevAppSettings>();
    }
}