using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Text.Json;
using SchnapsSchuss.Tests.Models.Entities;
using SchnapsSchuss.Tests.Models.Databases;

namespace SchnapsSchuss.Tests.ViewModels;

public class HomePageViewModel
{
    private bool _isAdmin;

    public Dictionary<string, string> _preferences;
    public bool IsAdmin { get; set; }
    public List<int> _personIds { get; set; }
    public bool _isFromPopUp = false;
    public List<Person> Persons { get; set; }

    private PersonDatabase PersonDatabase;
    private InvoiceDatabase InvoiceDatabase;
    private InvoiceItemDatabase InvoiceItemDatabase;

    public HomePageViewModel(PersonDatabase pDatabase, InvoiceDatabase iDatabase, InvoiceItemDatabase iiDatabase, Dictionary<string, string> preferences)
    {
        Persons = new List<Person>();
        _personIds = new List<int>();

        PersonDatabase = pDatabase;
        InvoiceDatabase = iDatabase;
        InvoiceItemDatabase = iiDatabase;

        _preferences = preferences;

        LoadPersonList();
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

    public async void LoadPersonList()
    {
        LoadIdsFromPreferences();

        List<Person> personsList = await PersonDatabase.GetPersonToIdsAsync(_personIds);
        foreach (Person person in personsList)
        {
            person.OpenInvoice = await InvoiceDatabase.GetOpenInvoiceForPerson(person.Id);
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
                List<InvoiceItem> items = await InvoiceItemDatabase.GetInvoiceItemsOfInvoiceAsync(person.OpenInvoice);
                person.OpenInvoice.InvoiceItems = items;
            }
        }
        Persons = new List<Person>(personsList);
    }


    private void StoreIdsToPreferences()
    {
        _personIds = _personIds.Distinct().ToList();
        string serialized = JsonSerializer.Serialize(_personIds);
        _preferences["PersonIds"] = serialized;
    }

    private void LoadIdsFromPreferences()
    {
        string stored;
        _preferences.TryGetValue("PersonIds", out stored);
        _personIds = string.IsNullOrEmpty(stored)
            ? new List<int>()
            : JsonSerializer.Deserialize<List<int>>(stored) ?? new List<int>();
    }


    public void ApplyQueryAttributes(Dictionary<string, object> query)
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