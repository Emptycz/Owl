using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Owl.Enums;
using Owl.Factories;
using Owl.Interfaces;
using Owl.Models;
using Owl.Models.Requests;

namespace Owl.ViewModels.Models;

public class GroupRequestVm : IRequestVm, INotifyPropertyChanged
{
    private string _name = string.Empty;

    public Guid Id { get; set; }
    public RequestNodeType Type => RequestNodeType.Group;
    public RequestAuth? Auth { get; set; }
    public ObservableCollection<IRequestVm> Children { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public GroupRequestVm(GroupRequest groupRequest)
    {
        Id = groupRequest.Id;
        Name = groupRequest.Name;
        // TODO: This needs to be mapped with a factory function
        Children = new ObservableCollection<IRequestVm>(groupRequest.Children.Select(RequestNodeVmFactory.GetRequestNodeVm));
    }

    public string GetIcon()
    {
        throw new NotImplementedException();
    }

    public IRequest ToRequest()
    {
        return new GroupRequest(this);
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
