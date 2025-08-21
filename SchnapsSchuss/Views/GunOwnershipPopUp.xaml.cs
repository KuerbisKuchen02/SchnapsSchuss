using CommunityToolkit.Maui.Views;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public partial class GunOwnershipPopUp : Popup<string>
{
    public GunOwnershipPopUp()
    {
        InitializeComponent();
        BindingContext = new GunOwnershipPopUpViewModel();

        Opened += HandlePopupOpened;
    }

    async void HandlePopupOpened(object? sender, EventArgs e)
    {
        // Delay for one second to ensure the user sees the previous text

    }
}