
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Models;
using DailyRegister.Services;

using System.Collections.ObjectModel;

using Contact = DailyRegister.Models.Contact;

namespace DailyRegister.ViewModels;

public partial class AddEditEventViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public AddEditEventViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private string _title = "Add Event";

    [ObservableProperty]
    private string _selectedType = "debt";

    [ObservableProperty]
    private ObservableCollection<Contact> _contacts = [];

    [ObservableProperty]
    private Contact? _selectedContact;

    [ObservableProperty]
    private string _amount = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private DateTime _eventDate = DateTime.Now;

    [ObservableProperty]
    private bool _showContact;

    [ObservableProperty]
    private bool _showAmount;

    [ObservableProperty]
    private bool _isDebt;

    public List<string> EventTypes { get; } = ["debt", "deposit", "expense", "product_entry", "general_note"];

    private AppEvent? _existingEvent;

    public async Task InitializeAsync()
    {
        var contacts = await _databaseService.GetContactsAsync();
        Contacts = new ObservableCollection<Contact>(contacts);
        UpdateFieldVisibility();
    }

    public void SetEvent(AppEvent? appEvent)
    {
        _existingEvent = appEvent;
        if (appEvent is not null)
        {
            Title = "Edit Event";
            SelectedType = appEvent.Type;
            SelectedContact = Contacts.FirstOrDefault(c => c.Id == appEvent.ContactId);
            Amount = appEvent.Amount?.ToString() ?? string.Empty;
            Description = appEvent.Description;
            EventDate = appEvent.EventDate;
            UpdateFieldVisibility();
        }
    }

    partial void OnSelectedTypeChanged(string value)
    {
        UpdateFieldVisibility();
    }

    private void UpdateFieldVisibility()
    {
        ShowContact = SelectedType != "general_note";
        ShowAmount = SelectedType is "debt" or "deposit" or "expense";
        IsDebt = SelectedType == "debt";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // Validate
        if (ShowContact && SelectedContact is null)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please select a contact.", "OK");
            return;
        }

        if (ShowAmount && !decimal.TryParse(Amount, out _))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter a valid amount.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Description) && SelectedType == "expense")
        {
            await Shell.Current.DisplayAlertAsync("Error", "Description is required for expenses.", "OK");
            return;
        }

        var appEvent = _existingEvent ?? new AppEvent();
        appEvent.Type = SelectedType;
        appEvent.ContactId = SelectedContact?.Id;
        appEvent.Amount = ShowAmount && decimal.TryParse(Amount, out var parsed) ? parsed : null;
        appEvent.Description = Description ?? string.Empty;
        appEvent.EventDate = EventDate;

        // Reset status if not a debt
        if (appEvent.Type != "debt")
        {
            appEvent.Status = null;
            appEvent.StatusNote = null;
        }
        else if (appEvent.Status is null)
        {
            appEvent.Status = "unpaid";
        }

        await _databaseService.SaveEventAsync(appEvent);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}