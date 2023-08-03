using ImFine.Client.Auth0;
using ImFine.Client.Services;
using ImFine.Client.ViewModels;
using ImFine.Client.Views;
using Microsoft.Extensions.Configuration;
using Plugin.LocalNotification;
using System.Reflection;

namespace ImFine.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification(config =>
                {
                    config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
                    {
                        ActionList = new HashSet<NotificationAction>(new List<NotificationAction>()
                        {
                            new NotificationAction(100)
                            {
                                Title="Yes",
                                Android=
                                {
                                    LaunchAppWhenTapped = false,
                                    IconName =
                                    {
                                        ResourceName = "i2"
                                    }
                                }
                            },
                            new NotificationAction(101)
                            {
                                Title="No",
                                Android =
                                {
                                    LaunchAppWhenTapped = false,
                                    IconName =
                                    {
                                        ResourceName = "i3"
                                    }
                                }
                            }
                        }
                        )
                    });
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddMemoryCache();
            // 👇 new code
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<GroupListPage>();
            builder.Services.AddSingleton<GroupOwnerPage>();
            builder.Services.AddSingleton<GroupMemberPage>();
            builder.Services.AddSingleton<GroupService>();
            builder.Services.AddSingleton<GroupListViewModel>();
            builder.Services.AddSingleton<GroupOwnerViewModel>();
            builder.Services.AddSingleton<IGroupService, GroupService>();
            builder.Services.AddSingleton<IHubService, HubService>();
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("ImFine.Client.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);
            var test = builder.Configuration["Auth0:Domain"];

            builder.Services.AddSingleton(new Auth0Client(new()
            {
                Domain = builder.Configuration["Auth0:Domain"],
                ClientId = builder.Configuration["Auth0:ClientId"],
                Scope = builder.Configuration["Auth0:Scope"],
                RedirectUri = builder.Configuration["Auth0:RedirectUri"],
                Audience = builder.Configuration["Auth0:Audience"],
            }));
            return builder
                .Build();
        }

        //        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        //        {
        //            CrossFirebaseCloudMessaging.Current.NotificationTapped += (sender, e) =>
        //            {
        //                Application.Current.Quit();
        //            };
        //            builder.ConfigureLifecycleEvents(events =>
        //            {
        //#if IOS
        //                events.AddiOS(iOS => iOS.FinishedLaunching((_, __) =>
        //                {
        //                    CrossFirebase.Initialize();
        //                    return false;
        //                }));
        //#else
        //            events.AddAndroid(android => android.OnCreate((activity, _) =>
        //                CrossFirebase.Initialize(activity)));
        //#endif
        //            });

        //            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        //            return builder;
        //        }
        //private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        //{
        //    return new CrossFirebaseSettings(isAuthEnabled: true, isCloudMessagingEnabled: true, isAnalyticsEnabled: true);
        //}
    }
}