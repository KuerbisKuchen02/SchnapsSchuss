using System.Collections.ObjectModel;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
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


    private ObservableCollection<Person> _persons;

    public ObservableCollection<Person> Persons
    {
        get => _persons;
        set => SetProperty(ref _persons, value);
    }

    public HomePageViewModel()
    {
        ManageButtonCommand = new Command(OnManageButtonClicked);
        LogOffButtonCommand = new Command(OnLogOffButtonClicked);
        AddPersonCommand = new Command(OnAddPersonButtonClick);
        PersonLeaveCommand = new Command<Person>(OnPersonLeaveCommand);
        PersonBookCommand = new Command<Person>(OnPersonBookCommand);

        Persons = new ObservableCollection<Person>();


        LoadPersonsFromDB();
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
        var parameters = new Dictionary<string, object>
        {
            { "alreadyThere", Persons.ToList() }
        };
        Shell.Current.GoToAsync(nameof(AddPersonPage), parameters);
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

    private async void LoadPersonsFromDB()
    {
        PersonDatabase personDatabase = new PersonDatabase();
        List<Person> personsList = await personDatabase.GetAllAsync();
        Persons = new ObservableCollection<Person>(personsList);
    }
}