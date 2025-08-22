using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;

namespace SchnapsSchuss.ViewModels;

public class AddPersonPageViewModel : BaseViewModel, IQueryAttributable
{
    public ICommand BackCommand { get; }
    public ICommand AddGuestCommand { get; }

    private PersonDatabase _personDB;
    public PersonDatabase PersonDB { get; set; }

    private String _searchText = string.Empty;

    public String SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilterTable(value);
            }
        }
    }

    private Person _selectedPerson;

    public Person SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            if (SetProperty(ref _selectedPerson, value))
            {
                OnPersonSelected();
            }
        }
    }

    public Collection<Person> Persons { get; set; }

    private ObservableCollection<Person> _filteredPersons = new ObservableCollection<Person>();

    public ObservableCollection<Person> FilteredPersons
    {
        get => _filteredPersons;
        set => SetProperty(ref _filteredPersons, value);
    }

    public AddPersonPageViewModel()
    {
        BackCommand = new Command(OnBack);
        AddGuestCommand = new Command(OnAddGuest);
        Persons = new Collection<Person>();
        FilteredPersons = Persons.ToObservableCollection();

        _personDB = new PersonDatabase();
        LoadPersons();
    }

    public async void LoadPersons()
    {
        List<Person> dbData = await _personDB.GetPersonsAsync();
        foreach (Person person in dbData)
        {
            Persons.Add(person);
        }

        FilteredPersons = Persons.ToObservableCollection();
    }

    public void FilterTable(string searchText)
    {
        ObservableCollection<Person> filtered = new ObservableCollection<Person>();
        foreach (Person p in Persons) {
            if (p.FirstName.ToLower().Contains(searchText.ToLower())) {
                filtered.Add(p);
            } else if (p.LastName.ToLower().Contains(searchText.ToLower())) {
                filtered.Add(p);
            } else if (p.DateOfBirth.ToString().ToLower().Contains(searchText.ToLower())) {
                filtered.Add(p);
            }
        }

        FilteredPersons = filtered;
    } 

    public void OnBack()
    {
        Shell.Current.GoToAsync("..");
    }

    public void OnAddGuest()
    {
        // TODO Add Pop Up Call here
    }

    public void OnPersonSelected()
    {
        Console.WriteLine("AAAAAAAAA");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        
    }
}
