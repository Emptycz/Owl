using System.Collections.Generic;
using Owl.Models;

namespace Owl.Importers;

public interface IImporter
{
    IEnumerable<OwlCollection> ImportRequests(string content);
}
