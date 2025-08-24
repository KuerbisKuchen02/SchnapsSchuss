using System.Collections.ObjectModel;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui;
using System.Diagnostics;
using System.Text.Json;

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

    private List<int> PersonIds { get; set; }
    private bool _isFromPopUp;

    private ObservableCollection<Person> _persons = [];

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
        PersonIds = [];

        LoadPersonList();
    }

    private static void OnManageButtonClicked()
    {
        Shell.Current.GoToAsync(nameof(AdminPage));
    }

    private static void OnLogOffButtonClicked()
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

    public void OnAppearing()
    {
        Debug.WriteLine("HomePage onAppearing is called");
        // Avoid reloading if coming back from a popup
        if (_isFromPopUp)
        {
            _isFromPopUp = false;
            return;
        }
        
        LoadPersonList();
    }

    public void OnDisappearing()
    {
        // Save the current state when the page is disappearing
        Debug.WriteLine("HomePage onDisAppearing is called");
        StoreIdsToPreferences();
    }
    
    private async void OnPersonLeaveCommand(Person person)
    {
        Console.WriteLine($"Person {person.FirstName} {person.LastName} is leaving.");

        var showGunOwnershipPopup = false;
        var openInvoice = await new InvoiceDatabase().GetOpenInvoiceForPerson(person.Id);

        // If there is an open invoice, show the LeavingPopUp first
        if (openInvoice is not null)
        {
            _isFromPopUp = true;
            var resultLeavingPopUp = await Shell.Current.ShowPopupAsync<bool>(new LeavingPopUp(new LeavingPopUpViewModel(openInvoice)), new PopupOptions());
            if (resultLeavingPopUp is { WasDismissedByTappingOutsideOfPopup: false, Result: true } )
            {
                showGunOwnershipPopup = true;
            }

        }
        else
        {
            showGunOwnershipPopup = true;
        }

        if (!showGunOwnershipPopup) return;
        
        _isFromPopUp = true;
        var result = await Shell.Current.ShowPopupAsync(new GunOwnershipPopUp(new GunOwnershipPopUpViewModel(person)), new PopupOptions());
        
        if (result.WasDismissedByTappingOutsideOfPopup) return;
    
        // update person list if person left
        PersonIds.Remove(person.Id);
        StoreIdsToPreferences();
        LoadPersonList();
    }

    private static void OnPersonBookCommand(Person person)
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
        
        var personDatabase = new PersonDatabase();
        var invoiceDatabase = new InvoiceDatabase();
        var invoiceItemDatabase = new InvoiceItemDatabase();
        
        var personsList = await personDatabase.GetPersonToIdsAsync(PersonIds);

        // Load open invoices for each person for total of invoice
        foreach (var person in personsList)
        {
            var invoice = await invoiceDatabase.GetOpenInvoiceForPerson(person.Id);

            if (invoice is null)
            {
                invoice = new Invoice
                {
                    PersonId = person.Id,
                    Date = DateTime.Now,
                    isPaidFor = true,
                    InvoiceItems = []
                };
            }
            else
            {
                invoice.InvoiceItems = await invoiceItemDatabase.GetInvoiceItemsOfInvoiceAsync(invoice);
            }

            person.OpenInvoice = invoice;
        }
        
        Persons = new ObservableCollection<Person>(personsList);
    }


    private void StoreIdsToPreferences()
    {
        PersonIds = PersonIds.Distinct().ToList(); 
        var serialized = JsonSerializer.Serialize(PersonIds);
        Preferences.Set("PersonIds", serialized);
    }

    private void LoadIdsFromPreferences()
    {
        var stored = Preferences.Get("PersonIds", string.Empty);
        PersonIds = string.IsNullOrEmpty(stored)
            ? []
            : JsonSerializer.Deserialize<List<int>>(stored) ?? [];
    }


    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Member", out var memberObj) && memberObj is Member member)
        {
            IsAdmin = member.Person.Role == RoleType.ADMINISTRATOR;
        }

        if (!query.TryGetValue("NewPerson", out var newPerson) || newPerson is not int newPersonId) return;
        
        PersonIds.Add(newPersonId);
        StoreIdsToPreferences();
    }
}