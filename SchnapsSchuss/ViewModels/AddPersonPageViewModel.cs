using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;

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

    private Collection<Person> _alreadyThere = new Collection<Person>();
    public Collection<Person> AlreadyThere
    {
        get => _alreadyThere;
        set => SetProperty(ref _alreadyThere, value);
    }

    private Collection<Person> _persons;

    public Collection<Person> Persons 
    {
        get => _persons;
        set
        {
            SetProperty(ref _persons, value);
            FilterTable(_searchText);
        }
        
    }

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
        List<Person> dbData = await _personDB.GetAllAsync();
        Collection<Person> temp = new Collection<Person>();
        foreach (Person person in dbData)
        {
            temp.Add(person);
        }
        Persons = temp;
    }

    public void FilterTable(string searchText)
    {
        // Filter out persons that are already in the "AlreadyThere" collection
        Collection<Person> collection_filter_already_there = new Collection<Person>();
        ObservableCollection<Person> collection_filtered = new ObservableCollection<Person>();

        foreach (Person p in Persons)
        {
            if (!AlreadyThere.Any(x => x.Id == p.Id))
            {
                collection_filter_already_there.Add(p);
            }
        }

        // Filter the rest of the persons based on the search text
        foreach (Person p in collection_filter_already_there) {
            if (p.FirstName.ToLower().Contains(searchText.ToLower())) {
                collection_filtered.Add(p);
            } else if (p.LastName.ToLower().Contains(searchText.ToLower())) {
                collection_filtered.Add(p);
            } else if (p.DateOfBirth.ToString().ToLower().Contains(searchText.ToLower())) {
                collection_filtered.Add(p);
            }
        }

        FilteredPersons = collection_filtered;
    } 

    public void OnBack()
    {
        Shell.Current.GoToAsync("///HomePage");
    }

    public void OnAddGuest()
    {
        AddGuestPopUp PopUp = new AddGuestPopUp(new AddGuestPopUpViewModel());
        Shell.Current.ShowPopup(PopUp, new PopupOptions());    
    }

    public void OnPersonSelected()
    {
        
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        AlreadyThere = (Collection<Person>) query["alreadyThere"];
    }
}
