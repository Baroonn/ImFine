using ImFine.Client.Auth0;
using System.Web;

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
        try
        {
            WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
        new WebAuthenticatorOptions()
        {
            Url = new Uri("https://imfine.azurewebsites.net/mobileauth/Google"),
            CallbackUrl = new Uri("myapp://"),
            PrefersEphemeralWebBrowserSession = true
        });

            string accessToken = authResult?.AccessToken;
            await SecureStorage.SetAsync("identity", authResult?.AccessToken);
            await SecureStorage.SetAsync("username", HttpUtility.UrlDecode(authResult?.Properties["email"]));
            await Shell.Current.GoToAsync(nameof(GroupListPage));
        }
        catch (Exception ex)
        {
            //Back to login
        }
    }
    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}