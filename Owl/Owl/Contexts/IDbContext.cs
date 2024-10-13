using LiteDB;
using Owl.Models;
using Owl.Models.Requests;
using Owl.Models.Variables;

namespace Owl.Contexts;

public interface IDbContext
{
    public ILiteCollection<IRequest> RequestNodes { get; }
    public ILiteCollection<IVariable> GlobalVariables { get; }
    public ILiteCollection<Environment> Environments { get; }
    public ILiteCollection<Settings> Settings { get; }
}
