using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyRegister.Helpers;
using DailyRegister.Models;
using DailyRegister.Services;
using System.Collections.ObjectModel;
using Contact = DailyRegister.Models.Contact;

namespace DailyRegister.ViewModels;

public partial class ContactsViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    public ContactsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [ObservableProperty]
    private ObservableCollection<Contact> _contacts = [];

    [ObservableProperty]
    private bool _isLoading;

    [RelayCommand]
    public async Task LoadContactsAsync()
    {
        IsLoading = true;
        var contacts = await _databaseService.GetContactsAsync();
        Contacts = new ObservableCollection<Contact>(contacts);
        IsLoading = false;
    }

    [RelayCommand]
    public async Task DeleteContactAsync(Contact contact)
    {
        await _databaseService.DeleteContactAsync(contact);
        Contacts.Remove(contact);
    }

    [RelayCommand]
    private async Task AddContactAsync()
    {
        await Shell.Current.GoToAsync("AddEditContact");
    }

    [RelayCommand]
    private async Task EditContactAsync(Contact contact)
    {
        await Shell.Current.GoToAsync("AddEditContact");
        // We'll pass data a simple way
        EditContactStore.ContactToEdit = contact;
    }

    [RelayCommand]
    private async Task ViewContactAsync(Contact contact)
    {
        await Shell.Current.GoToAsync($"ContactDetail?contactId={contact.Id}");
    }
}