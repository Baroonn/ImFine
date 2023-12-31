using ImFine.Server;
using ImFine.Server.Contracts;
using ImFine.Server.Extensions;
using ImFine.Server.Hubs;
using ImFine.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<ICosmosService, CosmosService>();
builder.Services.AddSignalR().AddAzureSignalR();
builder.Services.ConfigureSwagger();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// 1. Add Authentication Services\

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
    googleOptions.SaveTokens = true;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.RequireHttpsMetadata = false;
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/status"))
            {
                // Attempt to get a token from a query sting used by WebSocket
                var accessToken = context.Request.Query["access_token"];

                // If not present, extract the token from Authorization header

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    accessToken = context.Request.Headers["Authorization"].ToString()
                    .Replace("Bearer ", "");
                }

                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
var app = builder.Build();
// Configure the HTTP request pipeline.
ILogger<LogTest> logger = app.Services.GetRequiredService<ILogger<LogTest>>();
app.ConfigureExceptionHandler(logger);
if (app.Environment.IsProduction())
{
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "ImFine API v1");
});

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<StatusHub>("/status");

app.Run();
