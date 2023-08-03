using AndroidX.AppCompat.App;
using ImFine.Client.Auth0;
using ImFine.Client.Views;
using System.Diagnostics;

namespace ImFine.Client
{
    public partial class App : Application
    {
        public App(Auth0Client authClient)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            Routing.RegisterRoute(nameof(GroupOwnerPage), typeof(GroupOwnerPage));
            Routing.RegisterRoute(nameof(GroupMemberPage), typeof(GroupMemberPage));
            Routing.RegisterRoute(nameof(NewGroupPage), typeof(NewGroupPage));
            Routing.RegisterRoute(nameof(GroupListPage), typeof(GroupListPage));
            Routing.RegisterRoute(nameof(GroupSearchPage), typeof(GroupSearchPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

            MainPage = new AppShell();
        }





        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Created += (s, e) =>
            {
                Debug.WriteLine("PassXYZ.Vault.App: 1. Created event");
            };
            window.Activated += (s, e) =>
            {
                Debug.WriteLine("PassXYZ.Vault.App: 2. Activated event");
            };
            window.Deactivated += (s, e) =>
            {
                Debug.WriteLine("PassXYZ.Vault.App: 3. Deactivated event");
            };
            window.Stopped += (s, e) =>
            {
                Debug.WriteLine("PassXYZ.Vault.App: 4. Stopped event");
            };
            window.Resumed += (s, e) =>
            {
                Debug.WriteLine("PassXYZ.Vault.App: 5. Resumed event");
            };
            window.Destroying += (s, e) =>
            {
                Debug.WriteLine("PassXYZ.Vault.App: 6. Destroying event");
            };
            return window;
        }

        protected override void OnStart()
        {

        }
        protected override void OnSleep()
        {
            Debug.WriteLine("PassXYZ.Vault.App: OnSleep");
        }
        protected override void OnResume()
        {
            Debug.WriteLine("PassXYZ.Vault.App: OnResume");
        }
    }
}