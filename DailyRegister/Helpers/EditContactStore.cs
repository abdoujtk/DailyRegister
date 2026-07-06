using DailyRegister.Models;
using Contact = DailyRegister.Models.Contact;

namespace DailyRegister.Helpers;

public static class EditContactStore
{
    public static Contact? ContactToEdit { get; set; }
}