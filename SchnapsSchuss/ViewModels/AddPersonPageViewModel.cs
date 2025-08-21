using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class AddPersonPageViewModel : BaseViewModel
{
    public Command BackCommand { get; }

    public AddPersonPageViewModel()
    {
        BackCommand = new Command(OnBack);
    }

    public void OnBack()
    {
        Shell.Current.GoToAsync("..");
    }
}
