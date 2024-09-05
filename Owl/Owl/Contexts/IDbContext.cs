using LiteDB;
using Owl.Models;
using Owl.Models.Variables;
using RequestNode = Owl.Models.RequestNode;

namespace Owl.Contexts;

public interface IDbContext
{
    public ILiteCollection<RequestNode> RequestNodes { get; }
    public ILiteCollection<IVariable> GlobalVariables { get; }
    public ILiteCollection<Environment> Environments { get; }
    public ILiteCollection<Settings> Settings { get; }
}
