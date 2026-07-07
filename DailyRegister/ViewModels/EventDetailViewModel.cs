using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Models;
using DailyRegister.Services;


namespace DailyRegister.ViewModels;

public partial class EventDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public EventDetailViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private AppEvent? _appEvent;

    [ObservableProperty]
    private string _contactName = string.Empty;

    [ObservableProperty]
    private string _statusNote = string.Empty;

    [ObservableProperty]
    private bool _isDebt;

    [ObservableProperty]
    private bool _isPaid;

    [ObservableProperty]
    private bool _isTogglingStatus;

    public async Task LoadEventAsync(int eventId)
    {
        AppEvent = await _databaseService.GetEventAsync(eventId);
        if (AppEvent is null) return;

        IsDebt = AppEvent.Type == "debt";
        IsPaid = AppEvent.Status == "paid";
        StatusNote = AppEvent.StatusNote ?? string.Empty;

        if (AppEvent.ContactId.HasValue)
        {
            var contact = await _databaseService.GetContactAsync(AppEvent.ContactId.Value);
            ContactName = contact?.Name ?? "Unknown";
        }
    }

    [RelayCommand]
    private async Task ToggleStatusAsync()
    {
        if (AppEvent is null) return;

        IsTogglingStatus = true;

        if (IsPaid)
        {
            // Mark as unpaid — clear status
            AppEvent.Status = "unpaid";
            AppEvent.StatusNote = null;
            StatusNote = string.Empty;
            IsPaid = false;
        }
        else
        {
            // Mark as paid — show note prompt
            var note = await Shell.Current.DisplayPromptAsync(
                "Mark as Paid",
                "Add a note (e.g., how much paid, remaining):",
                "Save",
                "Cancel",
                placeholder: "Paid 200, remaining 300 moved to new debt");

            if (note is null)
            {
                IsTogglingStatus = false;
                return; // User cancelled
            }

            AppEvent.Status = "paid";
            AppEvent.StatusNote = note;
            StatusNote = note;
            IsPaid = true;
        }

        await _databaseService.SaveEventAsync(AppEvent);
        IsTogglingStatus = false;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}