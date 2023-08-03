using IdentityModel.Client;
using IdentityModel.OidcClient;

namespace ImFine.Client.Auth0
{
    public class Auth0Client
    {
        private readonly OidcClient oidcClient;
        private string audience;
        public Auth0Client(Auth0ClientOptions options)
        {
            oidcClient = new OidcClient(new OidcClientOptions
            {
                Authority = $"https://{options.Domain}",
                ClientId = options.ClientId,
                Scope = options.Scope,
                RedirectUri = options.RedirectUri,
                Browser = options.Browser
            });
            audience = options.Audience;
        }

        public IdentityModel.OidcClient.Browser.IBrowser Browser
        {
            get
            {
                return oidcClient.Options.Browser;
            }
            set
            {
                oidcClient.Options.Browser = value;
            }
        }

        public async Task<LoginResult> LoginAsync()
        {
            LoginRequest loginRequest = null;

            if (!string.IsNullOrEmpty(audience))
            {
                loginRequest = new LoginRequest
                {
                    FrontChannelExtraParameters = new Parameters(new Dictionary<string, string>()
            {
              {"audience", audience}
            })
                };
            }
            return await oidcClient.LoginAsync(loginRequest);
        }
    }
}
