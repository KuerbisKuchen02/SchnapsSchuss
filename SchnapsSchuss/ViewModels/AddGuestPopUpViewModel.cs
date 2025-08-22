using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels
{
    public class AddGuestPopUpViewModel : BaseViewModel
    {
        public ICommand CloseCommand { get; }
        public ICommand AddGuestCommand { get; }
        private string _guestFirstName;
        public string GuestFirstName
        {
            get => _guestFirstName;
            set => SetProperty(ref _guestFirstName, value);
        }

        private string _guestLastName;
        public string GuestLastName
        {
            get => _guestLastName;
            set => SetProperty(ref _guestLastName, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private DateTime _guestDateOfBirth = DateTime.Today;
        public DateTime GuestDateOfBirth
        {
            get => _guestDateOfBirth;
            set => SetProperty(ref _guestDateOfBirth, value);
        }

        private bool _hasGunownership = false;
        public bool HasGunownership
        {
            get => _hasGunownership;
            set => SetProperty(ref _hasGunownership, value);
        }



        public AddGuestPopUpViewModel()
        {
            CloseCommand = new Command(async () => await OnCloseClicked());
            AddGuestCommand = new Command(async () => await OnAddGuestClicked());
        }
        private async Task OnCloseClicked()
        {
            await Shell.Current.CurrentPage.ClosePopupAsync();
        }

        private async Task OnAddGuestClicked()
        {
            // Catch empty text fields
            if (string.IsNullOrWhiteSpace(GuestFirstName))
            {
                ErrorMessage = "Please enter a first name.";
                return;
            }
            else if (string.IsNullOrWhiteSpace(GuestLastName))
            {
                ErrorMessage = "Please enter a last name.";
                return;
            }

            // If both fields are filled, proceed with adding the guest

            Person newGuest = new Person
            {
                Id = 0,
                FirstName = GuestFirstName,
                LastName = GuestLastName,
                DateOfBirth = GuestDateOfBirth,
                Role = RoleType.GUEST,
                OwnsGunOwnershipCard = HasGunownership
            };

            try
            {
                var personDb = new PersonDatabase();
                int result = await personDb.SaveAsync(newGuest);

                // Open Homepage with the new guest added to persons
                Shell.Current.CurrentPage.ClosePopupAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding guest: {ex.Message}");
                ErrorMessage = "An error occurred while adding the guest. Please try again.";
            }

        }
    }
}
