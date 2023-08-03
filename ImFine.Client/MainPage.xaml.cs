using ImFine.Client.Auth0;

namespace ImFine.Client
{
    public partial class MainPage : TabbedPage
    {

        private readonly Auth0Client auth0Client;
        private string accessToken;

        public MainPage(Auth0Client auth0Client)
        {
            InitializeComponent();
            this.auth0Client = auth0Client;

        }

        //private async void OnApiCallClicked(object sender, EventArgs e)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        string ApiUrl = "/api/groups";
        //        httpClient.DefaultRequestHeaders.Authorization
        //                     = new AuthenticationHeaderValue("Bearer", accessToken);
        //        try
        //        {
        //            HttpResponseMessage response = await httpClient.GetAsync(ApiUrl);
        //            {
        //                string content = await response.Content.ReadAsStringAsync();
        //                await DisplayAlert("Info", content, "OK");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await DisplayAlert("Error", ex.Message, "OK");
        //        }
        //    }
        //}

        //private async void OnLoginClicked(object sender, EventArgs e)
        //{
        //    var loginResult = await auth0Client.LoginAsync();

        //    if (!loginResult.IsError)
        //    {
        //        Console.WriteLine(loginResult.AccessToken);
        //        UsernameLbl.Text = loginResult.User.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;
        //        UserPictureImg.Source = loginResult.User
        //          .Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

        //        LoginView.IsVisible = false;
        //        HomeView.IsVisible = true;
        //        accessToken = loginResult.IdentityToken;
        //    }
        //    else
        //    {
        //        await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        //    }
        //}

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            //await hubConnection.InvokeCoreAsync("UpdateGroupState", args: new[] { "myGroup", "stop", "message" });
            return;
        }
    }
}