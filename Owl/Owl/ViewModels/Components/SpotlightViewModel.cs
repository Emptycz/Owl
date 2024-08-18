using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Models;
using Owl.Repositories.RequestNode;
using Owl.Repositories.Spotlight;
using Owl.States;

namespace Owl.ViewModels.Components;

public partial class SpotlightViewModel : ViewModelBase
{
    [ObservableProperty] private string _filterText = string.Empty;
    [ObservableProperty] private IEnumerable<SpotlightNode> _items = [];
    [ObservableProperty] private SpotlightNode? _selectedItem;
    [ObservableProperty] private bool _isOpen = false;

    private readonly ISpotlightRepository _spotlightRepository;
    private readonly IRequestNodeRepository _nodeRepository;
    private readonly ISelectedNodeState _nodeState;

    public event EventHandler<bool>? IsOpenChanged;

    public SpotlightViewModel(
        ISpotlightRepository spotlightRepository,
        IRequestNodeRepository nodeRepository,
        ISelectedNodeState nodeState)
    {
        _spotlightRepository = spotlightRepository;
        _nodeState = nodeState;
        _nodeRepository = nodeRepository;
    }

    [RelayCommand]
    private void Close()
    {
        IsOpen = false;
    }

    [RelayCommand]
    private void UseSelectedItem()
    {
        if (SelectedItem is null) return;
        _nodeState.Current = _nodeRepository.Get(SelectedItem!.Id);
        Close();
    }

    partial void OnIsOpenChanged(bool value)
    {
        IsOpenChanged?.Invoke(this, value);
        if (!value) return;
        ResetComponent();
    }

    partial void OnFilterTextChanged(string value)
    {
        Items = _spotlightRepository.FindRequests(x => x.Name.Contains(value));
    }

    private void ResetComponent()
    {
        FilterText = string.Empty;
        SelectedItem = null;
        Items = _spotlightRepository.FindRequests();
    }
}
