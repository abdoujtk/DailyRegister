using DailyRegister.Models;

namespace DailyRegister.Helpers;

public static class EditEventStore
{
    public static AppEvent? EventToEdit { get; set; }
}