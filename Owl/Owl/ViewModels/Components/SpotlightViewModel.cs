using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Owl.Enums;
using Owl.Factories;
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
    [ObservableProperty] private bool _isOpen;

    private readonly ISpotlightRepository _spotlightRepository;
    private readonly IRequestNodeRepository _nodeRepository;
    private readonly IRequestNodeState _nodeState;

    public event EventHandler<bool>? IsOpenChanged;

    public SpotlightViewModel(
        ISpotlightRepository spotlightRepository,
        IRequestNodeRepository nodeRepository)
    {
        _spotlightRepository = spotlightRepository;
        _nodeState = RequestNodeState.Instance;
        _nodeRepository = nodeRepository;
    }

    [RelayCommand]
    private void Close()
    {
        IsOpenChanged?.Invoke(this, !IsOpen);
        IsOpen = false;
    }

    [RelayCommand]
    private void UseSelectedItem()
    {
        if (SelectedItem is null) return;
        var request = _nodeRepository.Get(SelectedItem!.Id);
        _nodeState.Current = request is null ? null : RequestNodeVmFactory.GetRequestNodeVm(request);
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
