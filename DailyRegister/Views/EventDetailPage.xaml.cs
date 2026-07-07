using DailyRegister.ViewModels;

namespace DailyRegister.Views;

[QueryProperty(nameof(EventId), "eventId")]
public partial class EventDetailPage : ContentPage
{
    private readonly EventDetailViewModel _viewModel;

    public EventDetailPage(EventDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private int _eventId;
    public int EventId
    {
        get => _eventId;
        set
        {
            _eventId = value;
            OnPropertyChanged();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadEventAsync(EventId);
    }
}