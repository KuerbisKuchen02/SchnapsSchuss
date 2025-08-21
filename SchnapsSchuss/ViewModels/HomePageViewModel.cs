using SchnapsSchuss.Models.Entities;
using System.Windows.Input;
using System.Collections.ObjectModel;
namespace SchnapsSchuss.ViewModels;

public class HomePageViewModel : BaseViewModel
{
    public ICommand ManageButtonCommand { get; }
    public ICommand LogOffButtonCommand { get; }
    public ICommand AddPersonCommand { get; }
    public ObservableCollection<Person> Persons { get; set; }

    public HomePageViewModel()
    {
        ManageButtonCommand = new Command(OnManageButtonClicked);
        LogOffButtonCommand = new Command(OnLogOffButtonClicked);
        AddPersonCommand = new Command(OnAddPersonButtonClick);

        Persons = new ObservableCollection<Person>
        {
            new Person { FirstName = "Max", LastName = "Mustermann", DateOfBirth=new DateTime(0) },
            new Person { FirstName = "Max", LastName = "Mustermann", DateOfBirth=new DateTime(0) }
        };
    }

    private void OnManageButtonClicked()
    {
     
    }

    private void OnLogOffButtonClicked()
    {

    }

    private void OnAddPersonButtonClick()
    {
        // Hier deine Logik
        Console.WriteLine("Add Person Button Clicked");
    }
}