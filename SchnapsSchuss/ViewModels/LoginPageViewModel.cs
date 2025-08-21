using System.Diagnostics;
using System.Windows.Input;
using SchnapsSchuss.Models.Databases;
using SchnapsSchuss.Models.Entities;
using SchnapsSchuss.Views;

namespace SchnapsSchuss.ViewModels;

public class LoginPageViewModel : BaseViewModel
{
    private MemberDatabase _MemberDatabase;
    private string _LoginButtonText = "Login";

    public ICommand LoginCommand { get; }

    public string LoginButtonText
    {
        get => this._LoginButtonText;
        set => this.SetProperty(ref this._LoginButtonText, value);
    }

    private string _username;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password;

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public LoginPageViewModel()
    {
        _MemberDatabase = new MemberDatabase();
        LoginCommand = new Command(async () => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        try
        {
            // Catch empty text fields
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password.";
                return;
            }

            // Check if the credentials are valid
            if (await ValidatePassword(Username, Password))
            {
                Shell.Current.GoToAsync(nameof(HomePage));
            }
            else
            {
                ErrorMessage = "You entered the wrong credentials";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login failed: {ex.Message}");
            ErrorMessage = "An error occurred during login.";
        }
    }

    private async Task<Boolean> ValidatePassword(String username, String password)
    {
        // TODO: Hash the password here?

        int memberCount = await _MemberDatabase.CheckIfUserExists(Username, Password);
        return memberCount != 0;
    }
}