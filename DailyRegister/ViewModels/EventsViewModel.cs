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

    [RelayCommand]
    public async Task LoadEventsAsync()
    {
        IsLoading = true;
        var events = await _databaseService.GetEventsAsync();
        Events = new ObservableCollection<AppEvent>(events);
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
}