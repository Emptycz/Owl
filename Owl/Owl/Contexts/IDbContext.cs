using LiteDB;
using Owl.Models;

namespace Owl.Contexts;

public interface IDbContext
{
    public ILiteCollection<RequestNode> RequestNodes { get; }
    public ILiteCollection<OwlVariable> GlobalVariables { get;  }
}
