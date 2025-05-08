using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Owl.Models.Variables;
using Owl.Repositories.Variable;
using Environment = Owl.Models.Environment;

namespace Owl.States;

public interface IEnvironmentState
{
	Environment? Current { get; set; }
	IEnumerable<IVariable> GetVariables();

	event EventHandler<Environment>? CurrentHasChanged;
	event EventHandler<bool> RequestedRefresh;

	void OnCurrentHasChanged(Environment? node);
}

public partial class EnvironmentState : ObservableObject, IEnvironmentState
{
	public event EventHandler<Environment>? CurrentHasChanged;
	public event EventHandler<bool>? RequestedRefresh;

	private IEnumerable<IVariable> _globalVariables;
	[ObservableProperty] private Environment? _current;

	public EnvironmentState(IVariableRepository repository)
	{
		_globalVariables = repository.GetAll();
	}

	public IEnumerable<IVariable> GetVariables()
	{
		return Current is null ? _globalVariables : _globalVariables.Concat(Current.Variables);
	}

	partial void OnCurrentChanged(Environment? value)
	{
		OnCurrentHasChanged(value);
	}

	public void OnCurrentHasChanged(Environment? node)
	{
		CurrentHasChanged?.Invoke(this, node!);
	}
}
