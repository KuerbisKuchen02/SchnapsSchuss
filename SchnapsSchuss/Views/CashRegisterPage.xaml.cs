using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public partial class CashRegisterPage : ContentPage
{
    public CashRegisterPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is CashRegisterViewModel vm &&
            vm.BackCommand.CanExecute(null))
        {
            vm.BackCommand.Execute(null);
        }

        return base.OnBackButtonPressed();
    }
}