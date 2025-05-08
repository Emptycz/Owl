using System;
using LiteDB;
using Owl.Enums;
using Owl.Models;
using Owl.Models.Requests;
using Owl.Models.Variables;
using Environment = Owl.Models.Environment;

namespace Owl.Contexts;

public interface IDbContext
{
    event EventHandler<DbEventOperation>? DbContextHasChanged;

    public ILiteCollection<IRequest> RequestNodes { get; }
    public ILiteCollection<IVariable> GlobalVariables { get; }
    public ILiteCollection<Environment> Environments { get; }
    public ILiteCollection<Settings> Settings { get; }

    /**
     * Switch database files on the fly.
     *
     * TODO: This needs to inform the repositories & states that source has changed so the UI can re-render
     */
    public void SwitchDatabase(string databasePath);
}
