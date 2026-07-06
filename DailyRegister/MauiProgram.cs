using CommunityToolkit.Maui;
using DailyRegister.Services;
using Microsoft.Extensions.Logging;
using DailyRegister.Views;
using DailyRegister.ViewModels;

namespace DailyRegister
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
            builder.Services.AddSingleton<DatabaseService>();

            // ViewModels
            builder.Services.AddSingleton<ContactsViewModel>();
            builder.Services.AddTransient<AddEditContactViewModel>();

            // Pages
            builder.Services.AddSingleton<ContactsPage>();
            builder.Services.AddTransient<AddEditContactPopup>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}