using System.Diagnostics;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class LoginPageViewModel : BaseViewModel
{
    private readonly MemberDatabase _memberDatabase;
    private string _loginButtonText = "Anmelden";

    public ICommand LoginCommand { get; }

    public string LoginButtonText
    {
        get => _loginButtonText;
        set => SetProperty(ref _loginButtonText, value);
    }

    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password = string.Empty;

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public LoginPageViewModel()
    {
        _ = new PersonDatabase(); 
        _memberDatabase = new MemberDatabase();
        _ = new InvoiceDatabase();
        _ = new InvoiceItemDatabase();
        _ = new ArticleDatabase();
        LoginCommand = new Command(async void () => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        try
        {
            // Catch empty text fields
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Bitte gib ein Benutzername und ein Passwort ein.";
                return;
            }

            var member = await ValidatePassword(Username, Password);

            // Check if the credentials are valid
            if (member != null)
            {
                // Clear credentials after login
                Username = "";
                Password = "";

                var parameters = new Dictionary<string, object>
                    {
                        { "Member", member},
                    };

                await Shell.Current.GoToAsync(nameof(HomePage), parameters);
            }
            else
            {
                ErrorMessage = "Passwort oder Benutzername ist falsch.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login failed: {ex.Message}");
            ErrorMessage = "Ein Fehler ist aufgetreten.";
        }
    }

        private async Task<Member?> ValidatePassword(string username, string password)
    {
        return await _memberDatabase.CheckIfUserExists(username, password);
    }
}