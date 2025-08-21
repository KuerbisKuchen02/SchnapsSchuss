using CommunityToolkit.Maui.Views;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.ViewModels;

namespace SchnapsSchuss.Views;

public partial class GunOwnershipPopUp : Popup<string>
{
    public GunOwnershipPopUp(GunOwnershipPopUpViewModel viewmodel)
    {
        BindingContext = viewmodel;
        InitializeComponent();

        Opened += HandlePopupOpened;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var person = (Person) query["person"];

    }

        async void HandlePopupOpened(object? sender, EventArgs e)
    {
        // Delay for one second to ensure the user sees the previous text

    }
}