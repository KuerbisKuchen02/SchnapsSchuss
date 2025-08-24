using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Entities;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

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
    
    public GunOwnershipPopUpViewModel(Person person)
    {
        CloseCommand = new Command(async void () => await OnOkayClicked());
        _person = person;
        SetLabelTextAndColor();
    }

    private void SetLabelTextAndColor()
    {
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
    
    private static async Task OnOkayClicked()
    {
        await Shell.Current.CurrentPage.ClosePopupAsync();
    }
}
