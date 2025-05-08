using System.Collections.Generic;
using System.Linq;
using Owl.Enums;
using Owl.Factories;
using Owl.ViewModels.Models;

namespace Owl.Models.Requests;

public class GroupRequest : RequestBase
{
    public IEnumerable<IRequest> Children { get; set; } = [];

    public GroupRequest()
    {

    }

    public GroupRequest(GroupRequestVm vm)
    {
        Id = vm.Id;
        Name = vm.Name;
        Children = vm.Children.Select(c => c.ToRequest());
    }
}
