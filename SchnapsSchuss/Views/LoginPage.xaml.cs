namespace SchnapsSchuss.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Disable flyout navigation to secure the application
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
    }
}