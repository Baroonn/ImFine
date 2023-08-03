using IdentityModel.Client;
using System.Net;

namespace ImFine.Client.Views;

public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        string accessToken = await SecureStorage.Default.GetAsync("identity");
        var baseUrl = "https://imfine.azurewebsites.net";

        if (accessToken != null)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.SetBearerToken(accessToken);

            var response = await httpClient.GetAsync($"{baseUrl}/api/groups");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                SecureStorage.Default.RemoveAll();
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            else
                await Shell.Current.GoToAsync(nameof(GroupListPage));
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}