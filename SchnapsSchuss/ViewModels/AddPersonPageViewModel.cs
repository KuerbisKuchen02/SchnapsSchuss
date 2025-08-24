using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Extensions;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace SchnapsSchuss.ViewModels;

public class AddPersonPageViewModel : BaseViewModel, IQueryAttributable
{
    public ICommand AddGuestCommand { get; }

    private readonly PersonDatabase _personDatabase;

    private string _searchText = string.Empty;

    public string SearchText
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

    private Person? _selectedPerson;

    public Person? SelectedPerson
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

    private Collection<Person> _alreadyThere = [];

    private Collection<Person> _persons = [];

    public Collection<Person> Persons 
    {
        get => _persons;
        set
        {
            SetProperty(ref _persons, value);
            FilterTable(_searchText);
        }
        
    }

    private ObservableCollection<Person> _filteredPersons = [];

    public ObservableCollection<Person> FilteredPersons
    {
        get => _filteredPersons;
        set => SetProperty(ref _filteredPersons, value);
    }

    public AddPersonPageViewModel()
    {
        AddGuestCommand = new Command(OnAddGuest);
        FilteredPersons = Persons.ToObservableCollection();
        _personDatabase = new PersonDatabase();
        LoadPersons();
    }
    
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _alreadyThere = (Collection<Person>) query["alreadyThere"];
    }

    private async void LoadPersons()
    {
        var dbData = await _personDatabase.GetAllAsync();
        Persons = new Collection<Person>(dbData);
    }

    private void FilterTable(string searchText)
    {
        // Filter out persons that are already in the "AlreadyThere" collection
        var collectionFilterAlreadyThere = new Collection<Person>();
        var collectionFiltered = new ObservableCollection<Person>();

        foreach (var p in Persons)
        {
            if (_alreadyThere.All(x => x.Id != p.Id)) collectionFilterAlreadyThere.Add(p);
        }

        // Filter the rest of the persons based on the search text
        foreach (var p in collectionFilterAlreadyThere) {
            if (p.FirstName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) 
                || p.LastName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) 
                || p.DateOfBirth.ToString(CultureInfo.CurrentCulture).Contains(searchText, StringComparison.CurrentCultureIgnoreCase)) {
                collectionFiltered.Add(p);
            }
        }

        FilteredPersons = collectionFiltered;
    } 

    private static void OnAddGuest()
    {
        var popUp = new AddGuestPopUp(new AddGuestPopUpViewModel());
        Shell.Current.ShowPopup(popUp, new PopupOptions());    
    }

    private async void OnPersonSelected()
    {
        // Open Homepage with the new guest added to persons
        var parameters = new Dictionary<string, object>
                {
                    { "NewPerson", SelectedPerson!.Id},
                };
        await Shell.Current.GoToAsync("..", parameters);   
    }
}
