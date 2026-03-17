using horseappspring26.Configuration;
using horseappspring26.Services;
using horseappspring26.ViewModels;
using horseappspring26.Views;
using Microsoft.Extensions.Logging;

namespace horseappspring26
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(new ApiSettings());
            builder.Services.AddSingleton<ITokenStore, TokenStore>();
            builder.Services.AddTransient<AuthTokenHandler>();

            builder.Services.AddHttpClient("HorseApi", (sp, client) =>
            {
                var settings = sp.GetRequiredService<ApiSettings>();
                client.BaseAddress = new Uri(settings.BaseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
#endif
                return handler;
            })
            .AddHttpMessageHandler<AuthTokenHandler>();

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IHorseApiService, HorseApiService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HorseListViewModel>();
            builder.Services.AddTransient<HorseDetailViewModel>();
            builder.Services.AddTransient<HorseEditViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HorseListPage>();
            builder.Services.AddTransient<HorseDetailPage>();
            builder.Services.AddTransient<HorseEditPage>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
