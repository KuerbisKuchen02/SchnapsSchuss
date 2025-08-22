using System.Collections.ObjectModel;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using static SQLite.TableMapping;
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

        Persons = new ObservableCollection<Person>();
    }

    private void OnManageButtonClicked()
    {
        Shell.Current.GoToAsync(nameof(AdminPage));
    }

    private void OnLogOffButtonClicked()
    {
        Shell.Current.GoToAsync(nameof(LoginPage));
    }

    private void OnAddPersonButtonClick()
    {
        var parameters = new Dictionary<string, object>
        {
            { "alreadyThere", Persons.ToList() }
        };
        Shell.Current.GoToAsync(nameof(AddPersonPage), parameters);
    }

    private void OnPersonLeaveCommand(Person person)
    {
        // TODO call leave Pop Up
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