using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Helpers;
using DailyRegister.Models;
using DailyRegister.Services;
using System.Collections.ObjectModel;
using Contact = DailyRegister.Models.Contact;

namespace DailyRegister.ViewModels;

public partial class ContactDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public ContactDetailViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private Contact? _contact;

    [ObservableProperty]
    private ObservableCollection<AppEvent> _contactEvents = [];

    [ObservableProperty]
    private int _unpaidDebtsCount;

    [ObservableProperty]
    private bool _hasUnpaidDebts;

    [ObservableProperty]
    private decimal _totalUnpaidAmount;

    public async Task LoadContactAsync(int contactId)
    {
        Contact = await _databaseService.GetContactAsync(contactId);
        if (Contact is null) return;

        var allEvents = await _databaseService.GetEventsAsync();
        var events = allEvents
            .Where(e => e.ContactId == contactId)
            .OrderByDescending(e => e.EventDate)
            .ToList();

        // Set contact names (not really needed here since they're all for this contact)
        ContactEvents = new ObservableCollection<AppEvent>(events);

        // Calculate unpaid debts
        var unpaid = events.Where(e => e.Type == "debt" && e.Status == "unpaid").ToList();
        UnpaidDebtsCount = unpaid.Count;
        HasUnpaidDebts = unpaid.Count > 0;
        TotalUnpaidAmount = unpaid.Sum(e => e.Amount ?? 0);
    }

    [RelayCommand]
    private async Task EditContactAsync()
    {
        if (Contact is null) return;
        EditContactStore.ContactToEdit = Contact;
        await Shell.Current.GoToAsync("AddEditContact");
    }

    [RelayCommand]
    private async Task DeleteContactAsync()
    {
        if (Contact is null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Contact",
            $"Are you sure you want to delete {Contact.Name}?",
            "Yes", "No");

        if (confirm)
        {
            await _databaseService.DeleteContactAsync(Contact);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}