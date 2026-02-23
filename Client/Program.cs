using Business.Abstract.ApiServices;
using Business.Security;
using Business.Services;
using Business.Services.ApiServices;
using Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = default!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<ILoggerService, FileLogger>();

                services.AddSingleton<ISessionContext, SessionContext>();
                services.AddTransient<AuthHeaderHandler>();

                services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                });

                services.AddHttpClient<IUserApiService, UserApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                })
                .AddHttpMessageHandler<AuthHeaderHandler>();

                services.AddHttpClient<IBarnApiService, BarnApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                })
                .AddHttpMessageHandler<AuthHeaderHandler>();

                services.AddHttpClient<IAnimalApiService, AnimalApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                })
                .AddHttpMessageHandler<AuthHeaderHandler>();

                services.AddHttpClient<IProductionApiService, ProductionApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                })
                .AddHttpMessageHandler<AuthHeaderHandler>();

                services.AddHttpClient<IInventoryApiService, InventoryApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                })
               .AddHttpMessageHandler<AuthHeaderHandler>();

                services.AddHttpClient<IWorkerApiService, WorkerApiService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7281/");
                })
              .AddHttpMessageHandler<AuthHeaderHandler>();

                services.AddTransient<Login>();
                services.AddTransient<MainForm>();

                services.AddTransient<Pages.CreateBarnForm>();
                services.AddTransient<Pages.AddUserForm>();
                services.AddTransient<Pages.HomePage>();
                services.AddTransient<Pages.PurchasePage>();
                services.AddTransient<Pages.ProductionPage>();
                services.AddTransient<Pages.InventoryPage>();
                services.AddTransient<Pages.PurchaseWorker>();
            });

            var host = builder.Build();
            ServiceProvider = host.Services;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show($"Beklenmedik bir hata: {((Exception)e.ExceptionObject).Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            var loginForm = ServiceProvider.GetRequiredService<Login>();
            Application.Run(loginForm);


            //ApplicationConfiguration.Initialize();

            //var builder = Host.CreateDefaultBuilder();

            //builder.ConfigureServices((context, services) =>
            //{
            //    services.AddSingleton<ILoggerService, FileLogger>();

            //    services.AddSingleton<ISessionContext, SessionContext>();
            //    services.AddTransient<AuthHeaderHandler>();

            //    services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
            //    {
            //        client.BaseAddress = new Uri("https://localhost:7281");
            //    });

            //    services.AddHttpClient("ApiClient", client =>
            //    {
            //        client.BaseAddress = new Uri("https://localhost:7281");
            //    })
            //    .AddHttpMessageHandler<AuthHeaderHandler>();

            //    services.AddScoped<IUserApiService, UserApiService>();
            //    services.AddScoped<IBarnApiService, BarnApiService>();

            //    services.AddTransient<Login>();
            //    services.AddTransient<MainForm>();

            //    //services.AddTransient<CreateBarnForm>();
            //    //services.AddTransient<AddUserForm>();
            //    //services.AddTransient<HomePage>();
            //    //services.AddTransient<PurchasePage>();
            //    //services.AddTransient<ProductionPage>();
            //    //services.AddTransient<InventoryPage>();
            //});

            //var host = builder.Build();
            //ServiceProvider = host.Services;

            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            //{
            //    MessageBox.Show($"Beklenmedik bir hata: {((Exception)e.ExceptionObject).Message}",
            //        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //};

            //var loginForm = ServiceProvider.GetRequiredService<Login>();
            //Application.Run(loginForm);
        }
    }
}