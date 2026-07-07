using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Helpers;
using DailyRegister.Models;
using DailyRegister.Services;
using System.Collections.ObjectModel;


namespace DailyRegister.ViewModels;

public partial class EventsViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public EventsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private ObservableCollection<AppEvent> _events = [];

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusFilter = "all"; // "all", "unpaid", "paid"

    [ObservableProperty]
    private ObservableCollection<AppEvent> _allEvents = [];

    public List<string> StatusFilters { get; } = ["all", "unpaid", "paid"];

    [RelayCommand]
    public async Task LoadEventsAsync()
    {
        IsLoading = true;
        var events = await _databaseService.GetEventsAsync();
        AllEvents = new ObservableCollection<AppEvent>(events);
        ApplyFilters();
        IsLoading = false;
    }

    [RelayCommand]
    private async Task AddEventAsync()
    {
        await Shell.Current.GoToAsync("AddEditEvent");
    }

    [RelayCommand]
    private async Task EditEventAsync(AppEvent appEvent)
    {
        EditEventStore.EventToEdit = appEvent;
        await Shell.Current.GoToAsync("AddEditEvent");
    }

    [RelayCommand]
    private async Task DeleteEventAsync(AppEvent appEvent)
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete",
            "Are you sure you want to delete this event?",
            "Yes", "No");

        if (confirm)
        {
            await _databaseService.DeleteEventAsync(appEvent);
            Events.Remove(appEvent);
        }
    }

    [RelayCommand]
    private async Task EventTappedAsync(AppEvent appEvent)
    {
        await Shell.Current.GoToAsync($"EventDetail?eventId={appEvent.Id}");
    }

   

    partial void OnStatusFilterChanged(string value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = AllEvents.AsEnumerable();

        if (StatusFilter != "all")
        {
            filtered = filtered.Where(e => e.Status == StatusFilter);
        }

        Events = new ObservableCollection<AppEvent>(filtered);
    }
}