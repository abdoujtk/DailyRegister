using DailyRegister.Helpers;
using DailyRegister.ViewModels;

namespace DailyRegister.Views;

public partial class AddEditContactPopup : ContentPage
{
    private readonly AddEditContactViewModel _viewModel;

    public AddEditContactPopup(AddEditContactViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var contact = EditContactStore.ContactToEdit;
        if (contact is not null)
        {
            _viewModel.SetContact(contact);
            EditContactStore.ContactToEdit = null; // Clear after use
        }
    }
}