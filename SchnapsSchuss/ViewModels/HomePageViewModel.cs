using SchnapsSchuss.Models.Entities;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SchnapsSchuss.Views;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui;
namespace SchnapsSchuss.ViewModels;

public class HomePageViewModel : BaseViewModel
{
    public ICommand ManageButtonCommand { get; }
    public ICommand LogOffButtonCommand { get; }
    public ICommand AddPersonCommand { get; }
    public ICommand PersonLeaveCommand { get; }
    public ICommand PersonBookCommand { get; }


    public ObservableCollection<Person> Persons { get; set; }

    public HomePageViewModel()
    {
        ManageButtonCommand = new Command(OnManageButtonClicked);
        LogOffButtonCommand = new Command(OnLogOffButtonClicked);
        AddPersonCommand = new Command(OnAddPersonButtonClick);
        PersonLeaveCommand = new Command<Person>(OnPersonLeaveCommand);
        PersonBookCommand = new Command<Person>(OnPersonBookCommand);

        Persons = new ObservableCollection<Person>
        {
            new Person { FirstName = "Max", LastName = "Mustermann", DateOfBirth=new DateTime(0) },
            new Person { FirstName = "Max", LastName = "Mustermann", DateOfBirth=new DateTime(0) }
        };
    }

    private void OnManageButtonClicked()
    {
        Shell.Current.GoToAsync(nameof(AdminPage));
    }

    private void OnLogOffButtonClicked()
    {
        Shell.Current.GoToAsync("..");
    }

    private void OnAddPersonButtonClick()
    {
        Shell.Current.GoToAsync(nameof(AddPersonPage));
    }

    private void OnPersonLeaveCommand(Person person)
    {
        Shell.Current.ShowPopupAsync(new LeavingPopUp(new LeavingPopUpViewModel(person)), new PopupOptions());
        Console.WriteLine($"Person {person.FirstName} {person.LastName} is leaving.");
    }

    private void OnPersonBookCommand(Person person)
    {
        var parameters = new Dictionary<string, object>
        {
            { "Person", person},
        };
        Shell.Current.GoToAsync(nameof(CashRegisterPage), parameters);
    }
}