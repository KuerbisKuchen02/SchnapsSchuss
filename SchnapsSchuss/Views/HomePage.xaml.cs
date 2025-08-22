using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Refresh the page when it appears
        if (BindingContext is HomePageViewModel viewModel)
        {
            viewModel.onAppearing();
        }
    }
}