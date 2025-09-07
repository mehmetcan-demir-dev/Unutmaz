using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;  // CommunityToolkit kullanımı
using Unutmaz.Services;      // Servislerimizi kullanmak için

namespace Unutmaz
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>(services => new App(services))
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<ShoppingCategoryService>();
            builder.Services.AddSingleton<ShoppingProductService>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
