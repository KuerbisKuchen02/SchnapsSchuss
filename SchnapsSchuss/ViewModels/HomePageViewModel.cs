using System.Collections.ObjectModel;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui;

namespace SchnapsSchuss.ViewModels;

public class HomePageViewModel : BaseViewModel, IQueryAttributable
{
    public ICommand ManageButtonCommand { get; }
    public ICommand LogOffButtonCommand { get; }
    public ICommand AddPersonCommand { get; }
    public ICommand PersonLeaveCommand { get; }
    public ICommand PersonBookCommand { get; }

    private bool _isAdmin;

    public bool IsAdmin
    {
        get => _isAdmin;
        set => SetProperty(ref _isAdmin, value);
    }

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
        Shell.Current.GoToAsync("///LoginPage");
    }

    private void OnAddPersonButtonClick()
    {
        var parameters = new Dictionary<string, object>
        {
            { "alreadyThere", Persons }
        };
        Shell.Current.GoToAsync("///AddPersonPage", parameters);
    }

    private async void OnPersonLeaveCommand(Person person)
    {
        Console.WriteLine($"Person {person.FirstName} {person.LastName} is leaving.");

        Invoice OpenInvoice = await new InvoiceDatabase().GetOpenInvoiceForPerson(person.Id);
        if (OpenInvoice == null)
        {
            Console.WriteLine($"No open invoice found for {person.FirstName} {person.LastName}.");
            Shell.Current.ShowPopupAsync(new GunOwnershipPopUp(new GunOwnershipPopUpViewModel(person)), new PopupOptions());
        }
        else
        {
            Shell.Current.ShowPopupAsync(new LeavingPopUp(new LeavingPopUpViewModel(OpenInvoice)), new PopupOptions());
        }
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

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Member member = (Member)query["Member"];

        IsAdmin = member.person.Role == RoleType.ADMINISTRATOR;
    }
}