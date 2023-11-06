using KWops.Mobile.ViewModels;

namespace KWops.Mobile.Views;

public partial class TeamsPage : ContentPage
{
    private readonly TeamsViewModel _viewModel;
    public TeamsPage(TeamsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();

    }
}