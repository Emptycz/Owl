using System.Collections.Generic;
using Owl.Models.Requests;

namespace Owl.Importers;

public interface IImporter
{
    IEnumerable<IRequest> ImportRequests(string content);
}
