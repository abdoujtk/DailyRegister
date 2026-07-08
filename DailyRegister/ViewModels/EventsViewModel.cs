using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Helpers;
using DailyRegister.Models;
using DailyRegister.Services;
using System.Collections.ObjectModel;
using System.Reflection;
using Contact = DailyRegister.Models.Contact;

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
    private ObservableCollection<AppEvent> _allEvents = [];

    [ObservableProperty]
    private bool _isLoading;

    // Filters
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _typeFilter = "all";

    [ObservableProperty]
    private string _statusFilter = "all";

    [ObservableProperty]
    private DateTime _dateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

    [ObservableProperty]
    private DateTime _dateTo = DateTime.Today;

    private List<Contact> _contacts = [];


    public List<string> TypeFilters { get; } = ["all", "debt", "deposit", "expense", "product_entry", "general_note"];
    public List<string> StatusFilters { get; } = ["all", "unpaid", "paid"];

    [RelayCommand]
    public async Task LoadEventsAsync()
    {
        IsLoading = true;
        var events = await _databaseService.GetEventsAsync();
        _contacts = await _databaseService.GetContactsAsync();

        // Set contact name on each event
        foreach (var evt in events)
        {
            if (evt.ContactId.HasValue)
            {
                evt.ContactName = _contacts
                    .FirstOrDefault(c => c.Id == evt.ContactId.Value)?.Name;
            }
        }

        AllEvents = new ObservableCollection<AppEvent>(events);
        ApplyFilters();
        IsLoading = false;
    }

    // When any filter changes, re-apply all filters
    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnTypeFilterChanged(string value) => ApplyFilters();
    partial void OnStatusFilterChanged(string value) => ApplyFilters();
    partial void OnDateFromChanged(DateTime value) => ApplyFilters();
    partial void OnDateToChanged(DateTime value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = AllEvents.AsEnumerable();

        
        // Search by text (matches description or contact name)
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.ToLower();
            filtered = filtered.Where(e =>
                e.Description.ToLower().Contains(search) ||
                (e.ContactId.HasValue && _contacts.Any(c =>
                    c.Id == e.ContactId.Value && c.Name.ToLower().Contains(search)))
            );
        }

        // Filter by type
        if (TypeFilter != "all")
        {
            filtered = filtered.Where(e => e.Type == TypeFilter);
        }

        // Filter by status
        if (StatusFilter != "all")
        {
            filtered = filtered.Where(e => e.Status == StatusFilter);
        }

        // Filter by date range
        filtered = filtered.Where(e => e.EventDate.Date >= DateFrom.Date);
        filtered = filtered.Where(e => e.EventDate.Date <= DateTo.Date);

        Events = new ObservableCollection<AppEvent>(filtered);
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
            AllEvents.Remove(appEvent);
            ApplyFilters();
        }
    }

    [RelayCommand]
    private async Task EventTappedAsync(AppEvent appEvent)
    {
        await Shell.Current.GoToAsync($"EventDetail?eventId={appEvent.Id}");
    }

   
    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        TypeFilter = "all";
        StatusFilter = "all";
        DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTo = DateTime.Today;
    }
}