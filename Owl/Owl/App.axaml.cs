using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Owl.Contexts;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.Repositories.Variable;
using Owl.Services;
using Owl.States;
using Owl.ViewModels;
using Owl.Views;

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

        // Register all the services needed for the application to run
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        // Creates a ServiceProvider containing services from the provided IServiceCollection
        var services = collection.BuildServiceProvider();

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
}

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IDbContext, LiteDbContext>();
        collection.AddSingleton<IRequestNodeRepository, LiteDbRequestNodeRepository>();
        collection.AddSingleton<IVariableRepository, LiteDbVariableRepository>();
        collection.AddSingleton<ISelectedNodeState, SelectedNodeState>();
        collection.AddSingleton<ISpotlightRepository, SpotlightRepository>();
        collection.AddTransient<IVariableResolver, DbVariableResolver>();
        collection.AddTransient<RequestNodeFormWindowViewModel>();
        collection.AddTransient<MainWindow>();
        collection.AddTransient<RequestViewModel>();
        collection.AddTransient<RequestView>();
    }
}
