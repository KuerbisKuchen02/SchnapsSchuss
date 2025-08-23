using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels
{
    public class GunOwnershipPopUpViewModel : BaseViewModel
    {

        public ICommand CloseCommand { get; }
        private string _labelText = "";
        private Color _labelTextColor = Colors.Black;
        private Person _person;
        public Person Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        public string LabelText
        {
            get => _labelText;
            set => SetProperty(ref _labelText, value);
        }

        public Color LabelTextColor
        {
            get => _labelTextColor;
            set => SetProperty(ref _labelTextColor, value);
        }


        public GunOwnershipPopUpViewModel(Person Person)
        {
            CloseCommand = new Command(async () => await OnOkayClicked());
            this._person = Person;
            setLabelTextAndColor();
        }

        private void setLabelTextAndColor()
        {
            if (Person == null)
            {
                LabelText = "Keine Person ausgewählt";
                LabelTextColor = Colors.Gray;
                return;
            }
            if (Person.OwnsGunOwnershipCard)
            {
                LabelText = "Besitzt Waffenbesitzschein";
                LabelTextColor = Colors.Green;
            }
            else
            {
                LabelText = "Besitzt KEINEN Waffenbesitzschein";
                LabelTextColor = Colors.Red;
            }
        }


        private async Task OnOkayClicked()
        {
            if (Shell.Current.CurrentPage is not null)
            {
                await Shell.Current.CurrentPage.ClosePopupAsync();               
            }
        }
    }
}
