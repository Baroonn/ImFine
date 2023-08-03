using ImFine.Client.Auth0;

namespace ImFine.Client.Views;

public partial class LoginPage : ContentPage
{
    private readonly Auth0Client auth0Client;
    public LoginPage(Auth0Client auth0Client)
    {
        InitializeComponent();
        this.auth0Client = auth0Client;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginResult = await auth0Client.LoginAsync();
        await SecureStorage.SetAsync("identity", loginResult.IdentityToken);
        await SecureStorage.SetAsync("username", loginResult.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value);
        await Shell.Current.GoToAsync(nameof(GroupListPage));
    }



    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}