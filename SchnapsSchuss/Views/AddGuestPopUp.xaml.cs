using CommunityToolkit.Maui.Views;
using SchnapsSchuss.ViewModels;
using System.Diagnostics;

namespace SchnapsSchuss.Views;

public partial class AddGuestPopUp : Popup<string>
{
	public AddGuestPopUp(AddGuestPopUpViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();

		Opened += HandlePopupOpened;
    }

    async void HandlePopupOpened(object? sender, EventArgs e)
    {
        // Delay for one second to ensure the user sees the previous text

    }
}