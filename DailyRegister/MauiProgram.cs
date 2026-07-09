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
            builder.Services.AddSingleton<EventsViewModel>();
            builder.Services.AddTransient<AddEditEventViewModel>();
            builder.Services.AddTransient<EventDetailViewModel>();
            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddTransient<ContactDetailViewModel>();

            // Pages
            builder.Services.AddSingleton<ContactsPage>();
            builder.Services.AddTransient<AddEditContactPopup>();
            builder.Services.AddSingleton<EventsPage>();
            builder.Services.AddTransient<AddEditEventPopup>();
            builder.Services.AddTransient<EventDetailPage>();
            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddTransient<ContactDetailPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}