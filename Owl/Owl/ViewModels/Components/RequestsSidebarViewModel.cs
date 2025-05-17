using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Owl.Contexts;
using Owl.Enums;
using Owl.EventModels;
using Owl.Factories;
using Owl.Importers;
using Owl.Interfaces;
using Owl.Models;
using Owl.Models.Requests;
using Owl.Repositories.Environment;
using Owl.Repositories.RequestNode;
using Owl.Services;
using Owl.States;
using Owl.ViewModels.Models;
using Serilog;
using Environment = Owl.Models.Environment;

namespace Owl.ViewModels.Components;

public partial class RequestsSidebarViewModel : ViewModelBase
{
	[ObservableProperty] private string _search = string.Empty;
	[ObservableProperty] private ObservableCollection<IRequestVm> _requests;
	[ObservableProperty] private IRequestNodeState _state;

	[ObservableProperty] private ObservableCollection<Environment> _environments;
	[ObservableProperty] private Environment? _selectedEnvironment;

	[ObservableProperty] private ObservableCollection<string> _collections;
	[ObservableProperty] private string? _selectedCollection;

	private readonly IRequestNodeRepository _repository;
	private readonly IEnvironmentRepository _environmentRepository;
	private readonly IDbContext _dbContext;

	public RequestsSidebarViewModel(IServiceProvider provider)
	{
		_environmentRepository = provider.GetRequiredService<IEnvironmentRepository>();
		var vars = _environmentRepository.GetAll().ToList();
		if (vars.Count == 0)
		{
			vars.Add(_environmentRepository.Add(new Environment { Name = "Default" }));
		}

		_environments = new ObservableCollection<Environment>(vars);
		_selectedEnvironment = _environments.FirstOrDefault();

		// _state = provider.GetRequiredService<IRequestNodeState>();
		_state = RequestNodeState.Instance;

		_repository = provider.GetRequiredService<IRequestNodeRepository>();
		_requests = new ObservableCollection<IRequestVm>(_repository.GetAll()
			.Select(RequestNodeVmFactory.GetRequestNodeVm));

		_repository.RepositoryHasChanged += OnRepositoryHasChanged;

		_collections = new ObservableCollection<string>(CollectionManager.CollectionFiles);
		_dbContext = provider.GetRequiredService<IDbContext>();
	}

	[RelayCommand]
	private void RefreshData(Guid requestNodeId)
	{
		var request = _repository.Get(requestNodeId);
		State.Current = request is null ? null : RequestNodeVmFactory.GetRequestNodeVm(request);
	}

	[RelayCommand]
	private void RemoveRequest(IRequestVm node)
	{
		_repository.Remove(node.Id);
		Requests.Remove(node);
	}

	[RelayCommand]
	private void AddNode()
	{
		var newNode = new HttpRequest
		{
			Name = "New Request",
			Method = HttpRequestMethod.Get,
			Body = string.Empty,
		};

		_repository.Add(newNode);
	}

	[RelayCommand]
	private void AddDirectory()
	{
		var newNode = new GroupRequest
		{
			Name = "New Directory",
		};

		_repository.Add(newNode);
	}

	[RelayCommand]
	private void RemoveRequestNode(IRequestVm node)
	{
		_repository.Remove(node.Id);
		Requests.Remove(node);
	}

	[RelayCommand]
	// TODO: This needs to be generalized
	private void DuplicateRequest(IRequestVm node)
	{
		var newNode = node.ToRequest();
		newNode.Id = Guid.NewGuid();

		_repository.Add(newNode);
	}

	partial void OnSelectedEnvironmentChanged(Environment? oldValue, Environment? newValue)
	{
		if (newValue is null) return;
		if (oldValue == newValue) return;

		var vars = _environmentRepository.Get(newValue.Id)?.Variables;
		if (vars is null) return;
		VariablesManager.AddVariables(vars, newValue.Name);
	}

	partial void OnSearchChanging(string value)
	{
		Requests = new ObservableCollection<IRequestVm>(_repository.Find(x => x.Name.Contains(value))
			.Select(RequestNodeVmFactory.GetRequestNodeVm));
	}

	private void OnRepositoryHasChanged(object? e, RepositoryEventObject<IRequest>? eventObject)
	{
		if (eventObject is null)
		{
			Log.Debug("OnRepositoryHasChanged was called with null eventObject");
			Requests = new ObservableCollection<IRequestVm>(_repository.GetAll()
				.Select(RequestNodeVmFactory.GetRequestNodeVm));

			return;
		};

		var nodeVm = eventObject.NewValue is not null ? RequestNodeVmFactory.GetRequestNodeVm(eventObject.NewValue) : null;

		if (nodeVm is null)
		{
			Log.Debug("OnRepositoryHasChanged was called with no new value, refreshing all requests");
			Requests = new ObservableCollection<IRequestVm>(_repository.GetAll()
				.Select(RequestNodeVmFactory.GetRequestNodeVm));

			return;
		}

		switch (eventObject.Operation)
		{
			case RepositoryEventOperation.AddedSingle:
				Requests.Add(nodeVm);
				return;
			case RepositoryEventOperation.RemovedSingle:
				Requests.Remove(nodeVm);
				break;
			case RepositoryEventOperation.UpdatedSingle:
			{
				Log.Warning(
					"TODO: Implement updating IRequests in RequestSideBarViewModel for OnRepositoryHasChanged!");
				break;
			}
			case RepositoryEventOperation.SourceChanged:
			case RepositoryEventOperation.AddedMany:
			default:
				Log.Debug("Refreshing all requests in RequestSideBarViewModel for OnRepositoryHasChanged because of {Operation}",
					eventObject.Operation);

				Requests = new ObservableCollection<IRequestVm>(_repository.GetAll()
					.Select(RequestNodeVmFactory.GetRequestNodeVm));
				break;
		}
	}

	partial void OnSelectedCollectionChanged(string? value)
	{
		if (string.IsNullOrEmpty(value)) return;

		Log.Debug("Switching database to {Database}", value);
		// FIXME: This needs to be way more resilient
		_dbContext.SwitchDatabase(Directory.GetCurrentDirectory() + $"/{value}");
	}

	// TEST::

	[RelayCommand]
	private void TestImport()
	{
		var content = File.ReadAllText("insomnia-export.json");
		var importer = new InsomniaV4Importer();
		var parsedData = importer.Parse(content);
		_dbContext.SwitchDatabase(Directory.GetCurrentDirectory() + "/Collections/insomnia-export.owl");
		_repository.Add(parsedData.Requests);
		// foreach (var request in parsedData.Requests)
		// {
		// 	if (request.Name == "invitation")
		// 	{
		// 		Console.WriteLine(JsonSerializer.Serialize(request as GroupRequest,
		// 			new JsonSerializerOptions{ WriteIndented = true }));
		// 	}
		//
		// 	Console.WriteLine("Adding request: " + request.Name);
		// 	// _repository.Add(request);
		// }
	}
}
