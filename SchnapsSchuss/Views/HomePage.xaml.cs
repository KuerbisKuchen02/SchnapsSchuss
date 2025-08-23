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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Handle any necessary cleanup when the page disappears
        if (BindingContext is HomePageViewModel viewModel)
        {
            // Add any necessary cleanup code here
            viewModel.onDisappearing();
        }
    }
}