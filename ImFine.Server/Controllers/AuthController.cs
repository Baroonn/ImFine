using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ImFine.Server.Controllers
{
    [Route("mobileauth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        const string callbackScheme = "myapp";
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string CreateToken(string email, DateTime expires_in)
        {
            var claims = new[]
            {
                new Claim("nickname", email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                                   claims: claims,
                                   expires: DateTime.Now.AddHours(8),
                                   signingCredentials: cred
                                   );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpGet("{scheme}")]
        public async Task Get([FromRoute] string scheme)
        {
            var auth = await Request.HttpContext.AuthenticateAsync(scheme);

            if (!auth.Succeeded
                || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
            {
                // Not authenticated, challenge
                await Request.HttpContext.ChallengeAsync(scheme);
            }
            else
            {
                var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
                var email = string.Empty;
                email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                DateTimeOffset date = (DateTimeOffset)auth.Properties.ExpiresUtc;
                if (email != null || date != null)
                {
                    await Request.HttpContext.ChallengeAsync(scheme);
                }
                var token = CreateToken(email, date.DateTime);

                // Get parameters to send back to the callback
                var qs = new Dictionary<string, string>
                {
                    { "access_token", token },
                    { "email", email }
                };

                // Build the result url
                var url = callbackScheme + "://#" + string.Join(
                    "&",
                    qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                // Redirect to final url
                Request.HttpContext.Response.Redirect(url);
            }
        }
    }
}
