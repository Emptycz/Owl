using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Owl.Contexts;
using Owl.Models;
using Owl.Repositories.Environment;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Settings;
using Owl.Repositories.Spotlight;
using Owl.Repositories.Variable;
using Owl.Services;
using Owl.Services.VariableResolvers;
using Owl.States;
using Owl.ViewModels;
using Owl.Views;
using Serilog;

namespace Owl;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug() // Change this based on your needs
            .WriteTo.Console() // Log to console
            .CreateLogger();

        // Register all the services needed for the application to run
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        // Creates a ServiceProvider containing services from the provided IServiceCollection
        var services = collection.BuildServiceProvider();

        CollectionManager.LoadCollections(Directory.GetCurrentDirectory() + "/Collections");
        // Register the default variables
        RegisterGlobalVariables(services.GetRequiredService<IVariableRepository>());

        // Check if there are any settings in the database, if not, create a new one
        var settingsRepository = services.GetRequiredService<ISettingsRepository>();
        if (!settingsRepository.GetAll().Any())
        {
            settingsRepository.Add(new Settings
            {
                RequestSettings = new RequestSettings
                {
                    FontSize = 14,
                    FontFamily = "Arial",
                }
            });
        }

        // Create an instance of MainWindow, passing in the resolved RequestViewModel
        var mainWindow = new MainWindow(services);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void RegisterGlobalVariables(IVariableRepository variableRepository)
    {
        // VariablesManager.AddVariables(variableRepository.GetAll());
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IVariableResolverFactory, VariableResolverFactory>();
        collection.AddSingleton<IDbContext, LiteDbContext>();
        collection.AddSingleton<IRequestNodeRepository, LiteDbRequestNodeRepository>();
        collection.AddSingleton<IVariableRepository, LiteDbVariableRepository>();
        collection.AddSingleton<IEnvironmentRepository, EnvironmentRepository>();
        collection.AddSingleton<ISpotlightRepository, SpotlightRepository>();
        collection.AddSingleton<ISettingsRepository, SettingsRepository>();

        // Register global states
        collection.AddSingleton<IRequestNodeState, RequestNodeState>();
        collection.AddSingleton<IEnvironmentState, EnvironmentState>();

        collection.AddTransient<RequestNodeFormWindowViewModel>();
        collection.AddTransient<MainWindow>();
        collection.AddTransient<RequestViewModel>();
        collection.AddTransient<RequestView>();
    }
}
