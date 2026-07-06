using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Models;
using DailyRegister.Services;
using System.Xml.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Contact = DailyRegister.Models.Contact;
using CommunityToolkit.Mvvm.Messaging;

namespace DailyRegister.ViewModels;

public partial class AddEditContactViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public AddEditContactViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private string _title = "Add Contact";

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    private Contact? _existingContact;

    public void SetContact(Contact? contact)
    {
        _existingContact = contact;
        if (contact is not null)
        {
            Title = "Edit Contact";
            Name = contact.Name;
            Phone = contact.Phone ?? string.Empty;
            Notes = contact.Notes ?? string.Empty;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Name is required.", "OK");
            return;
        }

        var contact = _existingContact ?? new Contact();
        contact.Name = Name;
        contact.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone;
        contact.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes;

        await _databaseService.SaveContactAsync(contact);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public void LoadContact(Contact? contact)
    {
        if (contact is not null)
            SetContact(contact);
    }
}