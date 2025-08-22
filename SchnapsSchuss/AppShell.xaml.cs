using SchnapsSchuss.Views;

namespace SchnapsSchuss;

public partial class AppShell : Shell
{
    static bool _routesRegistered = false;
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    static void RegisterRoutes()
    {
        if (_routesRegistered)
            return;
        Routing.RegisterRoute(nameof(AdminPage), typeof(AdminPage));
        Routing.RegisterRoute(nameof(CashRegisterPage), typeof(CashRegisterPage));
        Routing.RegisterRoute(nameof(CrudPage), typeof(CrudPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(AddPersonPage), typeof(AddPersonPage));
        _routesRegistered = true;
    }
}