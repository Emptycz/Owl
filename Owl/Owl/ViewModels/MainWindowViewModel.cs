using System;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.Repositories.Variable;
using Owl.States;
using Owl.ViewModels.Components;

namespace Owl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SpotlightViewModel SpotlightViewModel { get; }

    public MainWindowViewModel(IServiceProvider provider)
    {
        var variableRepository = provider.GetRequiredService<IVariableRepository>();
        var var = new OwlVariable { Key = "Test", Value = "TestValue" };
        variableRepository.Add(var);
        // variableRepository.DeleteAll();

        var vars = variableRepository.GetAll();
        foreach (var v in vars)
        {
            Console.WriteLine("This is the restored db type: {0}", v.GetType());
            Console.WriteLine(JsonSerializer.Serialize(v));
        }

        SpotlightViewModel = new SpotlightViewModel(
            provider.GetRequiredService<ISpotlightRepository>(),
            provider.GetRequiredService<IRequestNodeRepository>(),
            provider.GetRequiredService<ISelectedNodeState>()
        );
    }

    [RelayCommand]
    private void ToggleSpotlight()
    {
        SpotlightViewModel.IsOpen = !SpotlightViewModel.IsOpen;
    }
}
