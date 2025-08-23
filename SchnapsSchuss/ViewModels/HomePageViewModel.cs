using System.Collections.ObjectModel;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Maui.Core;

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

    private List<int> _personIds { get; set; }
    private bool _isFromPopUp = false;

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

        _personIds = new List<int>();

        LoadPersonList();
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
        Debug.WriteLine("AddPersonButton clicked");
        var parameters = new Dictionary<string, object>
        {
            { "alreadyThere", Persons }
        };
        Shell.Current.GoToAsync(nameof(AddPersonPage), parameters);
    }

    public async void onAppearing()
    {
        Debug.WriteLine("HomePage onAppearing is called");
        if (_isFromPopUp)
        {
            _isFromPopUp = false;
            return;
        }
        else
        {
            LoadPersonList();
        }
    }

    public async void onDisappearing()
    {
        Debug.WriteLine("HomePage onDisAppearing is called");
        StoreIdsToPreferences();
    }

    private async void OnPersonLeaveCommand(Person person)
    {
        Console.WriteLine($"Person {person.FirstName} {person.LastName} is leaving.");

        bool showGunOwnershipPopup = false;
        Invoice OpenInvoice = await new InvoiceDatabase().GetOpenInvoiceForPerson(person.Id);
        if (OpenInvoice is not null)
        {
            IPopupResult<bool> resultLeavingPopUp = await Shell.Current.ShowPopupAsync<bool>(new LeavingPopUp(new LeavingPopUpViewModel(OpenInvoice)), new PopupOptions());
            if (!resultLeavingPopUp.WasDismissedByTappingOutsideOfPopup && resultLeavingPopUp.Result )
            {
                showGunOwnershipPopup = true;
            }
            
        }
        else
        {
            showGunOwnershipPopup = true;
        }

        if (showGunOwnershipPopup)
        {
            _isFromPopUp = true;
            IPopupResult result = await Shell.Current.ShowPopupAsync(new GunOwnershipPopUp(new GunOwnershipPopUpViewModel(person)), new PopupOptions());
            if (!result.WasDismissedByTappingOutsideOfPopup)
            {
                _personIds.Remove(person.Id);
                StoreIdsToPreferences();
                LoadPersonList();
            }
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

    private async void LoadPersonList()
    {
        LoadIdsFromPreferences();

        PersonDatabase personDatabase = new PersonDatabase();
        List<Person> personsList = await personDatabase.GetPersonToIdsAsync(_personIds);
        foreach (Person person in personsList)
        {
            person.OpenInvoice = await new InvoiceDatabase().GetOpenInvoiceForPerson(person.Id);
            if (person.OpenInvoice is null)
            {
                person.OpenInvoice = new Invoice
                {
                    PersonId = person.Id,
                    Date = DateTime.Now,
                    isPaidFor = true,
                    InvoiceItems = null,
                };
            }
            else
            {
                List<InvoiceItem> items = await new InvoiceItemDatabase().GetInvoiceItemsOfInvoiceAsync(person.OpenInvoice);
                person.OpenInvoice.InvoiceItems = items;
            }
        }
        Persons = new ObservableCollection<Person>(personsList);
    }


    private void StoreIdsToPreferences()
    {
        _personIds = _personIds.Distinct().ToList(); 
        string serialized = JsonSerializer.Serialize(_personIds);
        Preferences.Set("PersonIds", serialized);
    }

    private void LoadIdsFromPreferences()
    {
        string stored = Preferences.Get("PersonIds", string.Empty);
        _personIds = string.IsNullOrEmpty(stored)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(stored) ?? new List<int>();
    }


    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Member", out var memberObj) && memberObj is Member member)
        {
            IsAdmin = member.Person.Role == RoleType.ADMINISTRATOR;
        }
        if (query.TryGetValue("NewPerson", out var newPerson) && newPerson is int newPersonId)
        {
            _personIds.Add(newPersonId);
            StoreIdsToPreferences();
        }
    }
}