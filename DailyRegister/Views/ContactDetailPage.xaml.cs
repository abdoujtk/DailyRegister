using DailyRegister.ViewModels;

namespace DailyRegister.Views;

[QueryProperty(nameof(ContactId), "contactId")]
public partial class ContactDetailPage : ContentPage
{
    private readonly ContactDetailViewModel _viewModel;

    public ContactDetailPage(ContactDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private int _contactId;
    public int ContactId
    {
        get => _contactId;
        set
        {
            _contactId = value;
            OnPropertyChanged();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadContactAsync(ContactId);
    }
}