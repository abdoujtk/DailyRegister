using DailyRegister.Helpers;
using DailyRegister.ViewModels;

namespace DailyRegister.Views;

public partial class AddEditEventPopup : ContentPage
{
    private readonly AddEditEventViewModel _viewModel;

    public AddEditEventPopup(AddEditEventViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();

        var eventToEdit = EditEventStore.EventToEdit;
        if (eventToEdit is not null)
        {
            _viewModel.SetEvent(eventToEdit);
            EditEventStore.EventToEdit = null;
        }
    }
}