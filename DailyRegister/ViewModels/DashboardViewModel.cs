using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Models;
using DailyRegister.Services;
using System.Collections.ObjectModel;

namespace DailyRegister.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public DashboardViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private int _totalUnpaidDebts;

    [ObservableProperty]
    private int _overdueDebts;

    [ObservableProperty]
    private int _totalEventsToday;

    [ObservableProperty]
    private ObservableCollection<AppEvent> _recentEvents = [];

    [ObservableProperty]
    private ObservableCollection<AppEvent> _overdueDebtList = [];

    [ObservableProperty]
    private bool _hasOverdueDebts;

    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        var allEvents = await _databaseService.GetEventsAsync();
        var contacts = await _databaseService.GetContactsAsync();
        var today = DateTime.Today;
        var weekAgo = today.AddDays(-7);

        // Total events today
        TotalEventsToday = allEvents.Count(e => e.EventDate.Date == today);

        // Unpaid debts
        var unpaidDebts = allEvents.Where(e => e.Type == "debt" && e.Status == "unpaid").ToList();
        TotalUnpaidDebts = unpaidDebts.Count;

        // Overdue debts (older than 7 days)
        var overdue = unpaidDebts.Where(e => e.EventDate.Date <= weekAgo).ToList();
        OverdueDebts = overdue.Count;
        HasOverdueDebts = overdue.Count > 0;

        // Set contact names on overdue debts
        foreach (var debt in overdue)
        {
            if (debt.ContactId.HasValue)
                debt.ContactName = contacts.FirstOrDefault(c => c.Id == debt.ContactId.Value)?.Name;
        }
        OverdueDebtList = new ObservableCollection<AppEvent>(overdue);

        // Recent events (last 10)
        var recent = allEvents
            .OrderByDescending(e => e.CreatedAt)
            .Take(10)
            .ToList();

        // Set contact names on recent events
        foreach (var evt in recent)
        {
            if (evt.ContactId.HasValue)
                evt.ContactName = contacts.FirstOrDefault(c => c.Id == evt.ContactId.Value)?.Name;
        }
        RecentEvents = new ObservableCollection<AppEvent>(recent);
    }

    [RelayCommand]
    private async Task AddEventAsync()
    {
        await Shell.Current.GoToAsync("AddEditEvent");
    }

    [RelayCommand]
    private async Task GoToEventsAsync()
    {
        await Shell.Current.GoToAsync("//EventsPage");
    }

    [RelayCommand]
    private async Task GoToUnpaidDebtsAsync()
    {
        // Navigate to events page — we'll pass a filter later, for now just go there
        await Shell.Current.GoToAsync("//EventsPage");
    }
}